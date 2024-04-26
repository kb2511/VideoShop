using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopModels.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartItems {  get; set; }
        public double OrderTotal { get; set; }
    }
}
