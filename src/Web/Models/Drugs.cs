using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AddDrugInfo
    {
        [Required]
        public string BrandId { get; set; }
        [Required]
        public EnumDrugClassification Classification { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Stock { get; set; }
        [Required]
        public double SafetyStock { get; set; }
        [Required]
        public double ReorderLevel { get; set; }
    }

    public class EditDrugInfo
    {
        [Required]
        public string DrugId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string BrandId { get; set; }
        [Required]
        public EnumDrugClassification Classification { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Stock { get; set; }
        [Required]
        public double SafetyStock { get; set; }
        [Required]
        public double ReorderLevel { get; set; }
    }
}
