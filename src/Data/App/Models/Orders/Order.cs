using Common.Extensions;
using Data.App.Models.Customers;
using Data.App.Models.Orders.OrderLineItems;
using Data.App.Models.Pharmacies;
using Data.App.Models.Users;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Orders
{
    public class Order
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public string Number { get; set; }
        public EnumOrderStatus OrderStatus { get; set; }
        public string CancelReason { get; set; }

        DateTime _orderDate = DateTime.UtcNow.Truncate();
        public DateTime OrderDate
        {
            get => _orderDate.AsUtc();
            set => _orderDate = value.Truncate();
        }

        DateTime _startPickupDate = DateTime.MaxValue;
        public DateTime StartPickupDate
        {
            get => _startPickupDate.AsUtc();
            set => _startPickupDate = value.Truncate();
        }

        DateTime _endPickupDate = DateTime.MaxValue;
        public DateTime EndPickupDate
        {
            get => _endPickupDate.AsUtc();
            set => _endPickupDate = value.Truncate();
        }

        public double GrossPrice { get; set; }
        //public double Deduction { get; set; }
        //public double NetPrice { get; set; }        

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<OrderLineItem> LineItems { get; set; } = new List<OrderLineItem>();
    }

    public static class OrderExtension
    {
        public static void ThrowIfNull(this Order me)
        {
            if (me == null)
                throw new ApplicationException("Order not found.");
        }

        public static void ThrowIfNullOrAlreadyUpdated(this Order me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("Order already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }
}
