using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AddOrderInfo
    {
        public string PharmacyId { get; set; }

        public List<OrderLineItemInfo> Items { get; set; }
    }

    public class OrderLineItemInfo
    {
        public string DrugId { get; set; }
        public double DrugQuantity { get; set; }
    }
}
