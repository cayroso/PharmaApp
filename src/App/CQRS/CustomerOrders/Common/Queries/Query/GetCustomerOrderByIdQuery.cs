using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;
using Data.Enums;

namespace App.CQRS.Orders.Common.Queries.Query
{
    public sealed class GetCustomerOrderByIdQuery : AbstractQuery<GetCustomerOrderByIdQuery.Order>
    {
        public GetCustomerOrderByIdQuery(string correlationId, string tenantId, string userId, string orderId)
            : base(correlationId, tenantId, userId)
        {
            OrderId = orderId;
        }
        public string OrderId { get; }

        public class Order
        {
            public string OrderId { get; set; }

            public string Number { get; set; }
            public Pharmacy Pharmacy { get; set; }
            public Customer Customer { get; set; }            
            public double GrossPrice { get; set; }

            public IEnumerable<Orderline> Lines { get; set; } = new List<Orderline>();
            public string Token { get; set; }
        }

        public class Pharmacy
        {
            public string PharmacyId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
        }

        public class Customer
        {
            public string CustomerId { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        public class Orderline
        {
            public string DrugName { get; set; }
            public double DrugPrice { get; set; }

            public string LineNumber { get; set; }
            public double Quantity { get; set; }
            public double ExtendedPrice { get; set; }
        }
    }
}
