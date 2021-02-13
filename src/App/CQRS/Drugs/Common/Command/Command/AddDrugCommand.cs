using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS.Drugs.Common.Command.Command
{
    public sealed class AddDrugCommand : AbstractCommand
    {
        public string PharmacyId { get; }
        public string DrugId { get; }
        public string BrandId { get; }
        public EnumDrugClassification Classification { get; }
        public string Name { get; }

        public double Price { get; }
        public double Stock { get; }
        public double SafetyStock { get; }
        public double ReorderLevel { get; }


        public AddDrugCommand(string correlationId, string tenantId, string userId,
            string pharmacyId, string drugId, string brandId, EnumDrugClassification classification, string name,
            double price, double stock, double safetyStock, double reorderLevel)
            : base(correlationId, tenantId, userId)
        {
            PharmacyId = pharmacyId;
            DrugId = drugId;
            BrandId = brandId;
            Classification = classification;
            Name = name;
            Price = price;
            Stock = stock;
            SafetyStock = safetyStock;
            ReorderLevel = reorderLevel;
        }
    }
}
