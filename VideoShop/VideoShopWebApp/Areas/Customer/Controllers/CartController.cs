using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopData.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebShopModels;
using System.Security.Claims;
using WebShopModels.Utility;
using WebShopModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.AspNetCore.Identity;

namespace VideoShopWebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductImageService _productImageService;
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;
		[BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IShoppingCartService shoppingCartService, IProductImageService productImageService, IOrderService orderService, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _shoppingCartService = shoppingCartService;
            _productImageService = productImageService;
            _orderService = orderService;
            _emailSender = emailSender;
            _userManager = userManager;

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

            ShoppingCartVM = new()
            {
                CartItems = shoppingCarts,
                Order = new()
                {
                    Total = GetOrderTotal(shoppingCarts)
                }
            };

            return View(ShoppingCartVM);
        }

		public async Task<IActionResult> Order()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				CartItems = await _shoppingCartService.GetAll(c => c.UserId == userId,
				                        includeProperties: new string[] { "Product" }),
				Order = new()
			};

			foreach (var cart in ShoppingCartVM.CartItems)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
			}

			ShoppingCartVM.Order.User = await _userManager.FindByIdAsync(userId);
			ShoppingCartVM.Order.BillingEmail = ShoppingCartVM.Order.User.Email;
			ShoppingCartVM.Order.BillingPhone = ShoppingCartVM.Order.User.PhoneNumber;
            ShoppingCartVM.Order.Status = StaticData.StatusPending;

            ShoppingCartVM.Order.Total = GetOrderTotal(ShoppingCartVM.CartItems);

			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Order")]
		public async Task<IActionResult> CreateOrder()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.CartItems = await _shoppingCartService.GetAll(u => u.UserId == userId, includeProperties: new string[] { "Product" });

			if (ShoppingCartVM.CartItems.Count() == 0)
			{
				RedirectToAction(nameof(Index));
			}

			ShoppingCartVM.Order.DateCreated = DateTime.Now;

			foreach (var cartItem in ShoppingCartVM.CartItems)
			{
				OrderProduct orderProduct = new OrderProduct()
				{
					OrderId = ShoppingCartVM.Order.Id,
					ProductId = cartItem.ProductId,
					Quantity = cartItem.Count,
					Price = (decimal)GetPriceBasedOnQuantity(cartItem)
				};

                orderProduct.Total = orderProduct.Quantity * orderProduct.Price;

				ShoppingCartVM.Order.OrderProducts.Add(orderProduct);
			}

			if (ModelState.IsValid)
			{
				await _orderService.Create(ShoppingCartVM.Order);
			}

            HttpContext.Session.SetInt32(StaticData.SessionCart, 0);

            return RedirectToAction(nameof(OrderConfirmation), new { id=ShoppingCartVM.Order.Id});

		}

		public async Task<IActionResult> OrderConfirmation(int id)
		{

			Order order = await _orderService.Get(u => u.Id == id, includeProperties: new string[] { "User" });

			List<ShoppingCart> shoppingCarts = (await _shoppingCartService.GetAll(u => u.UserId == order.UserId)).ToList();

			await _shoppingCartService.RemoveRange(shoppingCarts);

            return View(id);
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
