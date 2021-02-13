using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Pharmacies
{
    public class PharmacyStaff
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PharmacyStaffId { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public string StaffId { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
