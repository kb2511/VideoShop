using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebShopAlgebra.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
    }
}
