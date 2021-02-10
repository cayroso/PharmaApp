using Common.Extensions;
using Data.App.Models.Drugs;
using Data.App.Models.Orders.OrderLineItems;
using Data.App.Models.Pharmacies;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Medicines
{
    public class Drug
    {
        public string DrugId { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public EnumDrugPrescriptionStatus DrugPrescriptionStatus { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// True if this item's name is a proprietary/brand name (vs. generic name).
        /// </summary>
        public bool IsProprietary { get; set; }
        /// <summary>
        /// The generic name of this drug or supplement.
        /// </summary>
        public string GenericName { get; set; }
        /// <summary>
        /// Proprietary name given to the diet plan, typically by its originator or creator.
        /// </summary>
        public string ProprietaryName { get; set; }
        /// <summary>
        /// An active ingredient, typically chemical compounds and/or biologic substances.
        /// </summary>
        public string ActiveIngredient { get; set; }
        /// <summary>
        /// A dosage form in which this drug/supplement is available, e.g. 'tablet', 'suspension', 'injection'.
        /// </summary>
        public string DosageForm { get; set; }
        /// <summary>
        /// The unit in which the drug is measured, e.g. '5 mg tablet'.
        /// </summary>
        public string DrugUnit { get; set; }

        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsAvailable { get; set; }

        #region Inventory

        public double Stock { get; set; }
        public double SafetyStock { get; set; }
        public double ReorderLevel { get; set; }

        #endregion

        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        public bool Active { get; set; } = true;

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<DrugSubscription> CustomerSubscriptions { get; set; } = new List<DrugSubscription>();
        public virtual ICollection<DrugPrice> Prices { get; set; } = new List<DrugPrice>();
        public virtual ICollection<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
    }
}
