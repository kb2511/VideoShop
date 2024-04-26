using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Security.Claims;
using WebShopData.Interfaces;
using WebShopModels.Utility;

namespace VideoShopWebApp.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(StaticData.SessionCart) == null)
                {
                    var shoppingCarts = await _shoppingCartService.GetAll(u => u.UserId == claim.Value);
                    HttpContext.Session.SetInt32(StaticData.SessionCart, shoppingCarts.Count());
                }

                return View(HttpContext.Session.GetInt32(StaticData.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
