using Data.App.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public class Staff
    {
        public string StaffId { get; set; }
        public virtual User User { get; set; }
    }
}
