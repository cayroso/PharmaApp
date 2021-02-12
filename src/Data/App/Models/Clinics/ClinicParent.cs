using Data.App.Models.Parents;
using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public class ClinicParent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClinicParentId { get; set; }

        public string ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public string ParentId { get; set; }
        public virtual Parent Parent { get; set; }
    }
}
