using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopModels;

namespace WebShopData.Services
{
    public class ProductImageService : Service<ProductImage>, IProductImageService
    {
        private readonly ApplicationDbContext _context;
        public ProductImageService(ApplicationDbContext context) 
            : base(context)
        {
            _context = context;
        }

        public async Task Update(ProductImage image)
        {
            _context.ProductImage.Update(image);

            await _context.SaveChangesAsync();
        }
    }
}
