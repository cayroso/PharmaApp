using Common.Extensions;
using Data.App.Models.Brands;
using Data.App.Models.Drugs;
using Data.App.Models.Orders;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Pharmacies
{
    public class Pharmacy
    {
        public string PharmacyId { get; set; }

        public EnumPharmacyStatus PharmacyStatus { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }
        /// <summary>
        /// Example > Hours: Mo-Fr 10am-7pm Sa 10am-22pm Su 10am-21pm
        /// </summary>
        public string OpeningHours { get; set; }

        DateTime _dateCreated = DateTime.UtcNow.Truncate();
        public DateTime DateCreated
        {
            get => _dateCreated.AsUtc();
            set => _dateCreated = value.Truncate();
        }

        public string ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
        public virtual ICollection<Drug> Drugs { get; set; } = new List<Drug>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PharmacyReview> Reviews { get; set; } = new List<PharmacyReview>();
        public virtual ICollection<PharmacyStaff> Staffs { get; set; } = new List<PharmacyStaff>();
    }

    public static class PharmacyExtension
    {
        public static void ThrowIfNull(this Pharmacy me)
        {
            if (me == null)
                throw new ApplicationException("Pharmacy not found.");
        }
        public static void ThrowIfNullOrAlreadyUpdated(this Pharmacy me, string currentToken, string newToken)
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
