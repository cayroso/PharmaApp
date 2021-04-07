using Cayent.Core.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Command.Customer
{
    public sealed class CustomerPlaceOrderCommand : AbstractCommand
    {
        public string OrderId { get; }
        public string PharmacyId { get; }
        public IEnumerable<Line> Lines { get; }

        public CustomerPlaceOrderCommand(string correlationId, string tenantId, string userId,
            string orderId, string pharmacyId, IEnumerable<Line> lines)
            : base(correlationId, tenantId, userId)
        {
            OrderId = orderId;
            PharmacyId = pharmacyId;
            Lines = lines;
        }

        public class Line
        {
            public string DrugId { get; set; }
            public double Quantity { get; set; }
        }
    }
}
