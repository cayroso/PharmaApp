using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Queries.Query
{
    public sealed class GetDrugByIdQuery : AbstractQuery<GetDrugByIdQuery.Drug>
    {
        public string DrugId { get; }

        public GetDrugByIdQuery(string correlationId, string tenantId, string userId, string drugId)
               : base(correlationId, tenantId, userId)
        {
            DrugId = drugId;
        }

        public class Drug
        {
            public string DrugId { get; set; }

            public Pharmacy Pharmacy { get; set; }
            public DrupPrice Price { get; set; }
            public string Brand { get; set; }

            public EnumDrugClassification Classification { get; set; }
            public string ClassificationText => Classification.ToString();

            public string Name { get; set; }
            public bool IsAvailable { get; set; }
            public double Stock { get; set; }
            public double SafetyStock { get; set; }
            public double ReorderLevel { get; set; }
        }

        public class Pharmacy
        {
            public string PharmacyId { get; set; }
            public string Name { get; set; }

            public string Address { get; set; }
            public double GeoX { get; set; }
            public double GeoY { get; set; }
            public string OpeningHours { get; set; }
        }

        public class DrupPrice
        {
            public double Price { get; set; }
            public double SalePrice { get; set; }
        }
    }
}
