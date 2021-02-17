using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.CQRS.CustomerOrders.Common.Commands.Command.Customer;
using App.CQRS.CustomerOrders.Common.Commands.Command.Pharmacy;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Notifications;
using Data.App.Models.Orders;
using Data.App.Models.Orders.OrderLineItems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Handler
{
    public sealed class CustomerOrderCommonCommandHandler :
        ICommandHandler<CustomerArchiveOrderCommand>,
        ICommandHandler<CustomerCancelOrderCommand>,
        ICommandHandler<CustomerPlaceOrderCommand>,

        ICommandHandler<PharmacyAcceptOrderCommand>,
        ICommandHandler<PharmacyArchiveOrderCommand>,
        ICommandHandler<PharmacyCompleteOrderCommand>,
        ICommandHandler<PharmacyOrderReadyForPickupCommand>

    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        readonly NotificationService _notificationService;
        readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;

        public CustomerOrderCommonCommandHandler(
            AppDbContext appDbContext,
            ISequentialGuidGenerator sequentialGuidGenerator,
            IHubContext<OrderHub, IOrderClient> orderHubContext,
            NotificationService notificationService)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
            _orderHubContext = orderHubContext ?? throw new ArgumentNullException(nameof(orderHubContext));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        #region Customer

        async Task ICommandHandler<CustomerArchiveOrderCommand>.HandleAsync(CustomerArchiveOrderCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Archived;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            await _orderHubContext.Clients.Users(staffIds).CustomerSetOrderToArchived(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", "Order Archived",
                "The customer archived the order.", EnumNotificationType.Info, staffIds, null);


        }

        async Task ICommandHandler<CustomerCancelOrderCommand>.HandleAsync(CustomerCancelOrderCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Cancelled;
            data.CancelReason = command.Reason;

            data.AddTimeline(command.UserId, data.OrderStatus, command.Reason);

            await _appDbContext.SaveChangesAsync();

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            await _orderHubContext.Clients.Users(staffIds).CustomerCancelledOrder(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice,
                Notes = command.Reason
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-exclamation-circle", "Order Cancelled",
                "The customer cancelled the order.", EnumNotificationType.Warning, staffIds, null);
        }

        async Task ICommandHandler<CustomerPlaceOrderCommand>.HandleAsync(CustomerPlaceOrderCommand command)
        {
            var data = new Order
            {
                OrderId = command.OrderId,
                Number = _sequentialGuidGenerator.GenerateCode(12),
                CustomerId = command.UserId,
                PharmacyId = command.PharmacyId,
                OrderStatus = Data.Enums.EnumOrderStatus.Placed,
            };

            var drugIds = command.Lines.Select(e => e.DrugId).ToArray();

            var drugs = await _appDbContext.Drugs.Include(e => e.Prices).Where(e => drugIds.Contains(e.DrugId)).ToListAsync();

            var lineNumber = 0;

            data.LineItems = command.Lines.Select(e => new OrderLineItem
            {
                OrderId = data.OrderId,
                DrugId = e.DrugId,
                LineNumber = (++lineNumber).ToString("D2"),
                DrugPrice = drugs.First(d => d.DrugId == e.DrugId).Prices.First(),
                Quantity = e.Quantity,
                ExtendedPrice = drugs.First(d => d.DrugId == e.DrugId).Prices.First().Price * e.Quantity,
            }).ToList();

            data.GrossPrice = data.LineItems.Sum(e => e.ExtendedPrice);

            await _appDbContext.AddAsync(data);

            await _appDbContext.SaveChangesAsync();

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            await _orderHubContext.Clients.Users(staffIds).CustomerPlacedOrder(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-exclamation-circle", "Order Placed",
                $"A customer place an order with total price = {data.GrossPrice}.", EnumNotificationType.Warning, staffIds, null);
        }


        #endregion

        #region Pharmacy
        async Task ICommandHandler<PharmacyAcceptOrderCommand>.HandleAsync(PharmacyAcceptOrderCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Accepted;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.CustomerId };

            await _orderHubContext.Clients.Users(notifyIds).PharmacyAcceptedOrder(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info", "Order Accepted",
                "The pharmacy accepted your order.", EnumNotificationType.Info, notifyIds, null);
        }

        async Task ICommandHandler<PharmacyArchiveOrderCommand>.HandleAsync(PharmacyArchiveOrderCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Archived;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.CustomerId };

            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderToArchived(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info", "Order Archived",
                "The pharmacy archived your order.", EnumNotificationType.Info, notifyIds, null);
        }

        async Task ICommandHandler<PharmacyCompleteOrderCommand>.HandleAsync(PharmacyCompleteOrderCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Completed;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.CustomerId };

            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderToCompleted(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info", "Order Completed",
                "The pharmacy completed your order.", EnumNotificationType.Info, notifyIds, null);
        }

        async Task ICommandHandler<PharmacyOrderReadyForPickupCommand>.HandleAsync(PharmacyOrderReadyForPickupCommand command)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.ReadyForPickup;
            data.StartPickupDate = DateTime.UtcNow;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            var notifyIds = new[] { data.CustomerId };

            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderReadyForPickup(new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice
            });

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info", "Order Ready for Pickup",
                $"Your order can now be pickup from {data.StartPickupDate} until {data.EndPickupDate}.", EnumNotificationType.Info, notifyIds, null);
        }

        #endregion

    }
}
