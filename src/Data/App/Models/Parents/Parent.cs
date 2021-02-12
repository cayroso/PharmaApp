
using Data.App.Models.Clinics;
using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Parents
{
    public class Parent
    {
        public string ParentId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Child> Children { get; set; } = new List<Child>();
        public virtual ICollection<ClinicParent> Clinics { get; set; } = new List<ClinicParent>();
    }
}
