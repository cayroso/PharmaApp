using Cayent.Core.CQRS.Queries;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Queries.Query
{
    public sealed class SearchDrugByPharmacyQuery : AbstractPagedQuery<SearchDrugByPharmacyQuery.Drug>
    {
        public string PharmacyId { get; }

        public SearchDrugByPharmacyQuery(string correlationId, string tenantId, string userId,
            string pharmacyId, string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
            : base(correlationId, tenantId, userId, criteria, pageIndex, pageSize, sortField, sortOrder)
        {
            PharmacyId = pharmacyId;
        }

        public class Drug
        {
            public string DrugId { get; set; }

            public Pharmacy Pharmacy { get; set; }
            public DrupPrice Price { get; set; }
            public string BrandId { get; set; }
            public string BrandName { get; set; }
            public EnumDrugClassification Classification { get; set; }
            public string ClassificationText => Classification.ToString();

            public string Name { get; set; }
            public bool IsAvailable { get; set; }
        }

        public class Pharmacy
        {
            public string PharmacyId { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string MobileNumber { get; set; }
            public string OpeningHours { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            
        }

        public class DrupPrice
        {
            public double Price { get; set; }
            public double SalePrice { get; set; }
        }
    }
}
