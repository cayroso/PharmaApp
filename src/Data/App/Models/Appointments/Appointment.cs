using Common.Extensions;
using Data.App.Models.Clinics;
using Data.App.Models.Parents;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Appointments
{
    public class Appointment
    {
        public string AppointmentId { get; set; }

        public EnumAppointmentType Type { get; set; }
        public EnumAppointmentStatus Status { get; set; }

        public string ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public string ChildId { get; set; }
        public virtual Child Child { get; set; }


        DateTime _dateStart = DateTime.UtcNow.Truncate();
        public DateTime DateStart
        {
            get => _dateStart.AsUtc();
            set => _dateStart = value.Truncate();
        }

        DateTime _dateEnd = DateTime.UtcNow.Truncate();
        public DateTime DateEnd
        {
            get => _dateEnd.AsUtc();
            set => _dateEnd = value.Truncate();
        }

        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<ChildMedicalEntry> MedicalEntries { get; set; } = new List<ChildMedicalEntry>();
    }
}
