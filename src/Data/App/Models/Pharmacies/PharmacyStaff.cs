using System.ComponentModel.DataAnnotations.Schema;

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
