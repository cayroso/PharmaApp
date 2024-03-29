﻿using Data.App.Models.Drugs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.App.Models.Orders.OrderLineItems
{
    public class OrderLineItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderLineItemId { get; set; }

        public string OrderId { get; set; }
        public virtual Order Order { get; set; }

        public string DrugId { get; set; }
        public virtual Drug Drug { get; set; }

        public string DrugPriceId { get; set; }
        public virtual DrugPrice DrugPrice { get; set; }

        public string LineNumber { get; set; }
        public double Quantity { get; set; }
        public double ExtendedPrice { get; set; }
    }
}
