using Common.Extensions;
using Data.App.Models.Appointments;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Clinics
{
    public class Clinic
    {
        public string ClinicId { get; set; }

        public EnumPharmacyStatus PharmacyStatus { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }
        /// <summary>
        /// Example > Hours: Mo-Fr 10am-7pm Sa 10am-22pm Su 10am-21pm
        /// </summary>
        public string OpeningHours { get; set; }

        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<ClinicParent> Parents { get; set; } = new List<ClinicParent>();
        public virtual ICollection<ClinicStaff> Staffs { get; set; } = new List<ClinicStaff>();
       // public virtual ICollection<ClinicReview> Reviews { get; set; } = new List<ClinicReview>();
        //public virtual ICollection<ClinicStaff> Staffs { get; set; } = new List<ClinicStaff>();
    }
}
