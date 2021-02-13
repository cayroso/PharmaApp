using Common.Extensions;
using Data.App.Models.Brands;
using Data.App.Models.Drugs;
using Data.App.Models.Orders.OrderLineItems;
using Data.App.Models.Pharmacies;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Drugs
{
    public class Drug
    {
        public string DrugId { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public string BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        public EnumDrugClassification Classification { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// True if this item's name is a proprietary/brand name (vs. generic name).
        /// </summary>
        //public bool IsProprietary { get; set; }
        ///// <summary>
        ///// The generic name of this drug or supplement.
        ///// </summary>
        //public string GenericName { get; set; }
        ///// <summary>
        ///// Proprietary name given to the diet plan, typically by its originator or creator.
        ///// </summary>
        //public string ProprietaryName { get; set; }
        ///// <summary>
        ///// An active ingredient, typically chemical compounds and/or biologic substances.
        ///// </summary>
        //public string ActiveIngredient { get; set; }
        ///// <summary>
        ///// A dosage form in which this drug/supplement is available, e.g. 'tablet', 'suspension', 'injection'.
        ///// </summary>
        //public string DosageForm { get; set; }
        ///// <summary>
        ///// The unit in which the drug is measured, e.g. '5 mg tablet'.
        ///// </summary>
        //public string DrugUnit { get; set; }

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

    public static class DrugExtension
    {
        public static void ThrowIfNull(this Drug me)
        {
            if (me == null)
                throw new ApplicationException("Drug not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Drug me, string currentToken, string newToken)
        {
            me.ThrowIfNull();

            if (string.IsNullOrWhiteSpace(newToken))
                throw new ApplicationException("Anti-forgery token not found.");

            if (me.ConcurrencyToken != currentToken)
                throw new ApplicationException("Already updated by another user.");

            me.ConcurrencyToken = newToken;
        }
    }
}
