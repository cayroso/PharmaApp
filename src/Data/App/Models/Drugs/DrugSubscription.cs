using Data.App.Models.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Drugs
{
    public class DrugSubscription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DrugSubscriptionId { get; set; }

        public string DrugId { get; set; }
        public virtual Drug Drug { get; set; }

        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
