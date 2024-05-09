using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopModels
{
    public class OrderProduct
    {
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		[Column(TypeName = "decimal(9, 2)")]
		public decimal Quantity { get; set; }
		[Column(TypeName = "decimal(9, 2)")]
		public decimal Total { get; set; }
		[Column(TypeName = "decimal(9, 2)")]
		public decimal Price { get; set; }
		public virtual Product Product { get; set; }

	}
}
