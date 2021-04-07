using Cayent.Core.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Command.Pharmacy
{
    public sealed class PharmacyAcceptOrderCommand : AbstractCommand
    {
        public string OrderId { get; }
        public string Token { get; }

        public PharmacyAcceptOrderCommand(string correlationId, string tenantId, string userId,
            string orderId, string token)
            : base(correlationId, tenantId, userId)
        {
            OrderId = orderId;
            Token = token;
        }
    }
}
