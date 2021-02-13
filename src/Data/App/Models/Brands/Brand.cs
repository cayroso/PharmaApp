using Data.App.Models.Drugs;
using Data.App.Models.Pharmacies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Brands
{
    public class Brand
    {
        public string BrandId { get; set; }

        public string PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Drug> Drugs { get; set; } = new List<Drug>();

    }
}
