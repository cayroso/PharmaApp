using Data.App.Models.Users;

namespace Data.App.Models.Pharmacies
{
    public class Staff
    {
        public string StaffId { get; set; }
        public virtual User User { get; set; }
    }
}
