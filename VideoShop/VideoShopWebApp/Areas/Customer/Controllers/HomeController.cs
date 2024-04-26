using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebShopData.Interfaces;
using WebShopModels;
using WebShopModels.Utility;
using WebShopModels.ViewModels;

namespace VideoShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IShoppingCartService _shoppingCartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.CurrentCategory = "Filter by Category";

            IEnumerable<Product> productList = await _productService.GetAll(includeProperties: new string[] { "Category", "ProductImages" });

            if (categoryId != null)
            {
                var productIds = productList
                                .Where(p => p.CategoryId == categoryId)
                                .Select(p => p.Id);

                productList = productList.Where(p => productIds.Contains(p.Id)).ToList();

                var currentCategory = await _categoryService.Get(c => c.Id == categoryId);

                ViewBag.CurrentCategory = currentCategory?.Name;
            }

            var categories = await _categoryService.GetAll();

            ViewBag.Categories = categories.Select(c =>
            new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });


            return View(productList);
        }

        public async Task<IActionResult> Details(int productId)
        {
            Product product = await _productService.Get(p => p.Id == productId, includeProperties: new string[] { "Category", "ProductImages" });

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                Count = 1,
                ProductId = productId
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.UserId = userId;

            ShoppingCart cartFromDb = await _shoppingCartService.Get(u => u.UserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                await _shoppingCartService.Update(cartFromDb);
            }
            else
            {
                //add cart record
                await _shoppingCartService.Create(shoppingCart);

                var cart = await _shoppingCartService.GetAll(u => u.UserId == userId);

                HttpContext.Session.SetInt32(StaticData.SessionCart, cart.Count());
            }

            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
