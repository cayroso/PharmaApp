using Common.Extensions;
using Data.App.Models.Users;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Orders
{
    public class OrderTimeline
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderTimelineId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string OrderId { get; set; }
        public virtual Order Order { get; set; }

        public EnumOrderStatus Status { get; set; }

        public string Notes { get; set; }

        DateTime _dateTimeline = DateTime.UtcNow.Truncate();
        public DateTime DateTimeline
        {
            get => _dateTimeline.AsUtc();
            set => _dateTimeline = value.Truncate();
        }
    }
}
