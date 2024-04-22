using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopModels
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Duration (min)")]
        public int Duration { get; set; }
        [Required]
        [Display(Name = "Year Of Release")]
        [Range(1894, int.MaxValue)]
        public int YearOfRelease { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price For 1-3")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price For 4-10")]
        [Range(1, 1000)]
        public double PriceMoreThan3 { get; set; }

        [Required]
        [Display(Name = "Price For 10+")]
        [Range(1, 1000)]
        public double PriceMoreThan10 { get; set; }
 
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        [Display(Name = "Product Images")]
        public List<ProductImage> ProductImages { get; set; }
    }
}
