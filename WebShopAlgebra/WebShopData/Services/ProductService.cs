using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopModels;

namespace WebShopData.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task Update(Product product)
        {
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
