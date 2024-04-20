using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebShopData.Interfaces;
using WebShopModels;
using WebShopModels.ViewModels;

namespace WebShopAlgebra.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.CurrentCategory = "Filter by Category";

            IEnumerable<Product> productList = await _productService.GetAll(includeProperties: new string[] { "Category" }); ;

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

        public async Task<IActionResult> Details(int id)
        {
            Product product = await _productService.Get(p => p.Id == id, includeProperties: new string[] { "Category" });
            return View(product);
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
