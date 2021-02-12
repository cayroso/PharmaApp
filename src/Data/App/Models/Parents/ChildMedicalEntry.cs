using Common.Extensions;
using Data.App.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Parents
{
    public class ChildMedicalEntry
    {
        public string ChildMedicalEntryId { get; set; }

        public string AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }

        public string ChildId { get; set; }
        public virtual Child Child { get; set; }


        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }
    }
}
