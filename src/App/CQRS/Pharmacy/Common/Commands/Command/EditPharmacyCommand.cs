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
        public string PharmacyId { get; }
        public string Token { get; }
        public string Name { get; }
        public string PhoneNumber { get; }
        public string MobileNumber { get; }
        public string Email { get; }
        public EnumPharmacyStatus PharmacyStatus { get; }
        public string OpeningHours { get; }
        public string Address { get; }
        public double GeoX { get; }
        public double GeoY { get; }

        public EditPharmacyCommand(string correlationId, string tenantId, string userId,
            string pharmacyId, string token, string name, string phoneNumber, string mobileNumber, string email, EnumPharmacyStatus pharmacyStatus, string openingHours,
            string address, double geoX, double geoY)
            : base(correlationId, tenantId, userId)
        {
            PharmacyId = pharmacyId;
            Token = token;
            Name = name;
            PhoneNumber = phoneNumber;
            MobileNumber = mobileNumber;
            Email = email;
            PharmacyStatus = pharmacyStatus;
            OpeningHours = openingHours;
            Address = address;
            GeoX = geoX;
            GeoY = geoY;
        }
    }
}
