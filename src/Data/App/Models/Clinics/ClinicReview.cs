using Common.Extensions;
using Data.App.Models.Parents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public class ClinicReview
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClinicReviewId { get; set; }

        public string ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public string ParentId { get; set; }
        public virtual Parent Parent { get; set; }

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
