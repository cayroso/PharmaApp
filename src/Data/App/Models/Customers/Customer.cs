using Data.App.Models.Orders;
using Data.App.Models.Users;
using System.Collections.Generic;

namespace Data.App.Models.Customers
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public virtual User User { get; set; }


        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
