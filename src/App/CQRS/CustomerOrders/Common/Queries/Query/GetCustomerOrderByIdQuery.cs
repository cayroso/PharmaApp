using System;
using System.Collections.Generic;
using System.Text;
using Data.Enums;
using Cayent.Core.Common.Extensions;
using Cayent.Core.CQRS.Queries;

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

            public EnumOrderStatus Status { get; set; }
            public string StatusText => Status.ToString();

            public string Number { get; set; }
            public Pharmacy Pharmacy { get; set; }
            public Customer Customer { get; set; }
            public double GrossPrice { get; set; }
            public string Token { get; set; }


            DateTime _dateOrdered;
            public DateTime DateOrdered
            {
                get => _dateOrdered;
                set => _dateOrdered = value.AsUtc();
            }

            DateTime _dateStartPickup;
            public DateTime DateStartPickup
            {
                get => _dateStartPickup;
                set => _dateStartPickup = value.AsUtc();
            }

            DateTime _dateEndPickup;
            public DateTime DateEndPickup
            {
                get => _dateEndPickup;
                set => _dateEndPickup = value.AsUtc();
            }

            public IEnumerable<Orderline> Lines { get; set; } = new List<Orderline>();
            public IEnumerable<string> FileUploadUrls { get; set; } = new List<string>();
            public IEnumerable<OrderTimeline> Timelines { get; set; } = new List<OrderTimeline>();

        }

        public class Pharmacy
        {
            public string PharmacyId { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string MobileNumber { get; set; }
            public string Email { get; set; }
            public string OpeningHours { get; set; }
            public string Address { get; set; }
        }

        public class Customer
        {
            public string CustomerId { get; set; }
            public string UrlProfilePicture { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        public class Orderline
        {
            public string DrugName { get; set; }
            public double DrugPrice { get; set; }
            public EnumDrugClassification Classification { get; set; }
            public string ClassificationText => Classification.ToString();
            public string LineNumber { get; set; }
            public double Quantity { get; set; }
            public double ExtendedPrice { get; set; }
        }

        public class OrderTimeline
        {
            public EnumOrderStatus Status { get; set; }
            public string StatusText => Status.ToString();
            public string Notes { get; set; }
            public string User { get; set; }

            DateTime _dateTimeline;
            public DateTime DateTimeline
            {
                get => _dateTimeline;
                set => _dateTimeline = value.AsUtc();
            }
        }
    }
}
