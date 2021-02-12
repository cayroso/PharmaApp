using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public class ClinicStaff
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClinicStaffId { get; set; }

        public string ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public string StaffId { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
