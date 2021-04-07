using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.CQRS.CustomerOrders.Common.Commands.Command.Customer;
using App.CQRS.CustomerOrders.Common.Commands.Command.Pharmacy;
using App.Services;
using Cayent.Core.CQRS.Commands;
using Cayent.Core.CQRS.Services;
using Cayent.Core.Data.Notifications;
using Data.App.DbContext;

using Data.App.Models.Orders;
using Data.App.Models.Orders.OrderLineItems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
        ICommandHandler<PharmacyOrderReadyForPickupCommand>,
        ICommandHandler<PharmacyRejectedOrderCommand>

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

        async Task ICommandHandler<CustomerArchiveOrderCommand>.HandleAsync(CustomerArchiveOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Archived;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Archived: {data.Number}",
                $"<b>{data.Customer.User.FirstLastName}</b> archived his/her order <b>{data.Number}</b> <br/>with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b>.");

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);

            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", response.Title, response.Content, EnumNotificationType.Info, staffIds, null);

            await _orderHubContext.Clients.Users(staffIds).CustomerSetOrderToArchived(response);

            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<CustomerCancelOrderCommand>.HandleAsync(CustomerCancelOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Cancelled;
            data.CancelReason = command.Reason;

            data.AddTimeline(command.UserId, data.OrderStatus, command.Reason);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Cancelled: {data.Number}",
                $"<b>{data.Customer.User.FirstLastName}</b> cancelled his/her order <b>{data.Number}</b>." + (string.IsNullOrWhiteSpace(command.Reason) ? string.Empty : $"<br/>Due to <b>{command.Reason}</b>.")
            );

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-exclamation-circle", response.Title, response.Content, EnumNotificationType.Warning, staffIds, null);
            await _orderHubContext.Clients.Users(staffIds).CustomerCancelledOrder(response);

            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<CustomerPlaceOrderCommand>.HandleAsync(CustomerPlaceOrderCommand command, CancellationToken cancellationToken)
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

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.AddAsync(data);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Placed: {data.Number}",
                $"<b>{data.Customer.User.FirstLastName}</b> placed an order <b>{data.Number}</b> with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b>."
            );

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-exclamation-circle", response.Title, response.Content, EnumNotificationType.Warning, staffIds, null);
            await _orderHubContext.Clients.Users(staffIds).CustomerPlacedOrder(response);

            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }


        #endregion

        #region Pharmacy
        async Task ICommandHandler<PharmacyAcceptOrderCommand>.HandleAsync(PharmacyAcceptOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Accepted;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var notifyIds = new[] { data.CustomerId };
            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Accepted: {data.Number}",
                $"<b>{data.Pharmacy.Name}</b> accepted your order <b>{data.Number}</b> with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b>.");

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", response.Title, response.Content, EnumNotificationType.Info, notifyIds, null);
            await _orderHubContext.Clients.Users(notifyIds).PharmacyAcceptedOrder(response);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();
            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<PharmacyArchiveOrderCommand>.HandleAsync(PharmacyArchiveOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Archived;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var notifyIds = new[] { data.CustomerId };

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Archived: {data.Number}",
                $"<b>{data.Pharmacy.Name}</b> archived your order <b>{data.Number}</b> with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b>.");

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", response.Title, response.Content, EnumNotificationType.Info, notifyIds, null);
            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderToArchived(response);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();
            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<PharmacyCompleteOrderCommand>.HandleAsync(PharmacyCompleteOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Completed;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var notifyIds = new[] { data.CustomerId };

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Completed: {data.Number}",
                $"<b>{data.Pharmacy.Name}</b> completed your order <b>{data.Number}</b> with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b>.");

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", response.Title, response.Content, EnumNotificationType.Info, notifyIds, null);
            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderToCompleted(response);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();
            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<PharmacyOrderReadyForPickupCommand>.HandleAsync(PharmacyOrderReadyForPickupCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.ReadyForPickup;
            data.StartPickupDate = DateTime.UtcNow;
            data.EndPickupDate = DateTime.UtcNow.AddDays(2);

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var notifyIds = new[] { data.CustomerId };

            var phCI = new CultureInfo("fil-PH");

            var response = CreateResponse(data, $"Order Ready for Pickup: {data.Number}",
                $"Your order <b>{data.Number}</b> with total price of <b>{data.GrossPrice.ToString("C", phCI)}</b> in <b>{data.Pharmacy.Name}</b><br/>" +
                $"can now be pickup starting from <b>{data.StartPickupDate}</b> until <b>{data.EndPickupDate}</b>.");

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-info-circle", response.Title, response.Content, EnumNotificationType.Info, notifyIds, null);
            await _orderHubContext.Clients.Users(notifyIds).PharmacySetOrderReadyForPickup(response);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();
            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        async Task ICommandHandler<PharmacyRejectedOrderCommand>.HandleAsync(PharmacyRejectedOrderCommand command, CancellationToken cancellationToken)
        {
            var data = await _appDbContext.Orders.FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            data.ThrowIfNullOrAlreadyUpdated(command.Token, _sequentialGuidGenerator.NewId());

            data.OrderStatus = Data.Enums.EnumOrderStatus.Rejected;
            data.CancelReason = command.Reason;

            data.AddTimeline(command.UserId, data.OrderStatus, string.Empty);

            await _appDbContext.SaveChangesAsync();

            data = await _appDbContext.Orders
                .Include(e => e.Pharmacy)
                .Include(e => e.Customer)
                    .ThenInclude(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OrderId == command.OrderId);

            var notifyIds = new[] { data.CustomerId };


            var response = CreateResponse(data, $"Order Rejected: {data.Number}",
                $"<b>{data.Pharmacy.Name}</b> rejected your order <b>{data.Number}</b>." + (string.IsNullOrWhiteSpace(command.Reason) ? string.Empty : $"<br/>Due to <b>{command.Reason}</b>.")
                );

            await _notificationService.DeleteNotificationByReferenceId(data.OrderId);
            await _notificationService.AddNotification(data.OrderId, "fas fa-fw fa-exclamation-circle", response.Title, response.Content, EnumNotificationType.Warning, notifyIds, null);
            await _orderHubContext.Clients.Users(notifyIds).PharmacyRejectedOrder(response);

            var staffIds = await _appDbContext.PharmacyStaffs.Where(e => e.PharmacyId == data.PharmacyId).Select(e => e.StaffId).ToArrayAsync();
            var allNotifyIds = staffIds.Append(data.CustomerId).ToArray();
            await _orderHubContext.Clients.Users(allNotifyIds).OrderUpdated(data.OrderId);
        }

        #endregion

        Response CreateResponse(Order data, string title, string content)
        {
            var response = new Response
            {
                OrderId = data.OrderId,
                OrderNumber = data.Number,
                CustomerId = data.Customer.CustomerId,
                CustomerName = data.Customer.User.FirstLastName,

                PharmacyId = data.Pharmacy.PharmacyId,
                PharmacyName = data.Pharmacy.Name,
                TotalPrice = data.GrossPrice,

                Title = title,
                Content = content
            };

            return response;
        }
    }
}
