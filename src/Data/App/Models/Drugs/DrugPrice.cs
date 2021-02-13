using Common.Extensions;
using Data.App.Models.Orders.OrderLineItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Drugs
{
    public class DrugPrice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DrugPriceId { get; set; }

        public string DrugId { get; set; }
        public virtual Drug Drug { get; set; }

        public double Cogs { get; set; }
        public double Price { get; set; }
        public double SalePrice { get; set; }

        DateTime _saleStart = DateTime.MaxValue;
        public DateTime SaleStart
        {
            get => _saleStart.AsUtc();
            set => _saleStart = value.Truncate();
        }

        DateTime _saleEnd = DateTime.MaxValue;
        public DateTime SaleEnd
        {
            get => _saleEnd.AsUtc();
            set => _saleEnd = value.Truncate();
        }

        public uint LoyaltyPoints { get; set; }

        public bool Active { get; set; } = true;

        public virtual ICollection<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
    }
}
