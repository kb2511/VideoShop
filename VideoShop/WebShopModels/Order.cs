using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace WebShopModels
{
    public class Order
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Created")]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Total price is required.")]
        public double Total { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string BillingFirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string BillingLastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email address is not valid.")]
        [DisplayName("Email")]
        public string BillingEmail { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(50)]
        [DisplayName("Phone Number")]
        public string BillingPhone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50)]
        [DisplayName("Address")]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50)]
        [DisplayName("Country")]
        public string BillingCountry { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [StringLength(20)]
        [DisplayName("Zip Code")]
        public string BillingZipCode { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Message { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public IdentityUser User { get; set; }

		[ForeignKey("OrderId")]
		public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
	}
}
