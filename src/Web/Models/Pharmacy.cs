using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class EditPharmacyInfo
    {
        [Required]
        public string PharmacyId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }

        [Required]
        public EnumPharmacyStatus PharmacyStatus { get; set; }
        [Required]
        public string OpeningHours { get; set; }
        [Required]
        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }
    }
}
