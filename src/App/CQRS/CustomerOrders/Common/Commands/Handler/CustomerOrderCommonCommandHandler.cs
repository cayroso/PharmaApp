using App.CQRS.CustomerOrders.Common.Commands.Command;
using App.Services;
using Data.App.DbContext;
using Data.App.Models.Orders;
using Data.App.Models.Orders.OrderLineItems;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Handler
{
    public sealed class CustomerOrderCommonCommandHandler :
        ICommandHandler<AddCustomerOrderCommand>
    {
        readonly AppDbContext _appDbContext;
        readonly ISequentialGuidGenerator _sequentialGuidGenerator;
        public CustomerOrderCommonCommandHandler(AppDbContext appDbContext, ISequentialGuidGenerator sequentialGuidGenerator)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _sequentialGuidGenerator = sequentialGuidGenerator ?? throw new ArgumentNullException(nameof(sequentialGuidGenerator));
        }

        async Task ICommandHandler<AddCustomerOrderCommand>.HandleAsync(AddCustomerOrderCommand command)
        {
            var order = new Order
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

            order.LineItems = command.Lines.Select(e => new OrderLineItem
            {
                OrderId = order.OrderId,
                DrugId = e.DrugId,
                LineNumber = (++lineNumber).ToString("##"),
                DrugPrice = drugs.First(d => d.DrugId == e.DrugId).Prices.First(),
                Quantity = e.Quantity,
                ExtendedPrice = drugs.First(d => d.DrugId == e.DrugId).Prices.First().Price * e.Quantity,
            }).ToList();

            order.GrossPrice = order.LineItems.Sum(e => e.ExtendedPrice);

            await _appDbContext.AddAsync(order);

            await _appDbContext.SaveChangesAsync();
        }
    }
}
