using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Pharmacy.Common.Commands.Command
{
    public sealed class EditPharmacyCommand : AbstractCommand
    {
        public string PharmacyId { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public EnumPharmacyStatus PharmacyStatus { get; set; }
        public string OpeningHours { get; set; }
        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }

        public EditPharmacyCommand(string correlationId, string tenantId, string userId,
            string pharmacyId, string token, string name, EnumPharmacyStatus pharmacyStatus, string openingHours,
            string address, double geoX, double geoY)
            : base(correlationId, tenantId, userId)
        {
            PharmacyId = pharmacyId;
            Token = token;
            Name = name;
            PharmacyStatus = pharmacyStatus;
            OpeningHours = openingHours;
            Address = address;
            GeoX = geoX;
            GeoY = geoY;
        }
    }
}
