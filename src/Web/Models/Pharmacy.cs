using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
