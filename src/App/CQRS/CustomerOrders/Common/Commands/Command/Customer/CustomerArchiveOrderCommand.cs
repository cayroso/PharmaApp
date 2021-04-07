﻿using Cayent.Core.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.CustomerOrders.Common.Commands.Command.Customer
{
    public sealed class CustomerArchiveOrderCommand : AbstractCommand
    {
        public string OrderId { get; }
        public string Token { get; }

        public CustomerArchiveOrderCommand(string correlationId, string tenantId, string userId,
            string orderId, string token)
            : base(correlationId, tenantId, userId)
        {
            OrderId = orderId;
            Token = token;
        }
    }
}
