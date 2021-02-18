using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;
using Data.Common;
using Data.Enums;

namespace App.CQRS.Orders.Common.Queries.Query
{
    public sealed class SearchOrderQuery : AbstractPagedQuery<SearchOrderQuery.Order>
    {
        public string CustomerId { get; }
        public string PharmacyId { get; }
        public EnumOrderStatus OrderStatus { get; }

        public SearchOrderQuery(string correlationId, string tenantId, string userId, string customerId, string pharmacyId, EnumOrderStatus orderStatus,
            string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
            : base(correlationId, tenantId, userId, criteria, pageIndex, pageSize, sortField, sortOrder)
        {
            CustomerId = customerId;
            PharmacyId = pharmacyId;
            OrderStatus = orderStatus;
        }

        public class Order
        {
            public string OrderId { get; set; }

            public EnumOrderStatus Status { get; set; }
            public string StatusText => Status.ToString();

            public string Number { get; set; }
            public Pharmacy Pharmacy { get; set; }
            public Customer Customer { get; set; }
            public double GrossPrice { get; set; }
            public int NumberOfItems { get; set; }

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

    }
}
