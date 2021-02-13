using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Pharmacy.Common.Queries.Query
{
    public sealed class GetPharmacyByIdQuery : AbstractQuery<GetPharmacyByIdQuery.Pharmacy>
    {
        public string PharmacyId { get; }

        public GetPharmacyByIdQuery(string correlationId, string tenantId, string userId, string pharmacyId)
               : base(correlationId, tenantId, userId)
        {
            PharmacyId = pharmacyId;
        }
        public class Pharmacy
        {
            public string PharmacyId { get; set; }
            public string Token { get; set; }

            public EnumPharmacyStatus PharmacyStatus { get; set; }
            public string PharmacyStatusText => PharmacyStatus.ToString();

            public string Name { get; set; }
            public string Address { get; set; }
            public double GeoX { get; set; }
            public double GeoY { get; set; }            
            public string OpeningHours { get; set; }

        }
    }
}
