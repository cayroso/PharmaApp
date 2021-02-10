using Common.Extensions;
using Data.App.Models.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Pharmacies
{
    public class PharmacyReview
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PharmacyReviewId { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }


        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }
    }
}
