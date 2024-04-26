using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopData.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebShopModels;
using System.Security.Claims;
using WebShopModels.Utility;
using WebShopModels.ViewModels;

namespace VideoShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductImageService _productImageService;
        private readonly IEmailSender _emailSender;

        public CartController(IShoppingCartService shoppingCartService, IProductImageService productImageService, IEmailSender emailSender)
        {
            _shoppingCartService = shoppingCartService;
            _productImageService = productImageService;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCarts = await _shoppingCartService.GetAll(u => u.UserId == userId, includeProperties: new string[] { "Product" });

            IEnumerable<ProductImage> productImages = await _productImageService.GetAll();

            foreach (var cart in shoppingCarts)
            {
                cart.Product.ProductImages = productImages.Where(i => i.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
            }

            ShoppingCartVM shoppingCartVM = new()
            {
                CartItems = shoppingCarts,
                OrderTotal = GetOrderTotal(shoppingCarts)
            };

            return View(shoppingCartVM);
        }

        public async Task<IActionResult> AddOne(int? cartId)
        {
            if(cartId != null && cartId != 0)
            {
                var shoppingCart = await _shoppingCartService.Get(c => c.Id == cartId, includeProperties: new string[] { "Product" });
                shoppingCart.Count++;
                await _shoppingCartService.Update(shoppingCart);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveOne(int? cartId)
        {
            if (cartId != null && cartId != 0)
            {
                var shoppingCart = await _shoppingCartService.Get(c => c.Id == cartId, includeProperties: new string[] { "Product" });

                if(shoppingCart.Count <= 1)
                {
                    await _shoppingCartService.Delete(shoppingCart);

                    var allUserCarts = await _shoppingCartService.GetAll(c => c.UserId == shoppingCart.UserId);

                    HttpContext.Session.SetInt32(StaticData.SessionCart, allUserCarts.Count() - 1);
                }
                else
                {
                    shoppingCart.Count--;
                    await _shoppingCartService.Update(shoppingCart);
                }              
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int cartId)
        {
            var shoppingCart = await _shoppingCartService.Get(c => c.Id == cartId);
            await _shoppingCartService.Delete(shoppingCart);

            var allUserCarts = await _shoppingCartService.GetAll(c => c.UserId == shoppingCart.UserId);
            HttpContext.Session.SetInt32(StaticData.SessionCart, allUserCarts.Count());

            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 3)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 10)
            {
                return shoppingCart.Product.PriceMoreThan3;
            }
            else
            {
                return shoppingCart.Product.PriceMoreThan10;
            }
        }

        private double GetOrderTotal(IEnumerable<ShoppingCart> shoppingCart)
        {
            double total = 0;

            foreach (var cart in shoppingCart)
            {
                total += cart.Price * cart.Count;
            }

            return total;
        }
    }
}
