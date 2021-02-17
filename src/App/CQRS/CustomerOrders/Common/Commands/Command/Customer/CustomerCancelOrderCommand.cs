using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Command.Customer
{
    public sealed class CustomerCancelOrderCommand : AbstractCommand
    {
        public string OrderId { get; }
        public string Token { get; }
        public string Reason { get; }

        public CustomerCancelOrderCommand(string correlationId, string tenantId, string userId,
            string orderId, string token, string reason)
            : base(correlationId, tenantId, userId)
        {
            OrderId = orderId;
            Token = token;
            Reason = reason;
        }
    }
}
