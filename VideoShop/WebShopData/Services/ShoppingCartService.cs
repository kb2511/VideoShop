using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopModels;

namespace WebShopData.Services
{
    public class ShoppingCartService : Service<ShoppingCart>, IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartService(ApplicationDbContext context) 
            : base(context)
        {
            _context = context;
        }

        public async Task Update(ShoppingCart shoppingCart)
        {
            _context.ShoppingCart.Update(shoppingCart);
            await _context.SaveChangesAsync();
        }
    }
}
