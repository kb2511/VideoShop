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
            var productDb = _context.Product.FirstOrDefault(p => p.Id == product.Id);
            if (productDb != null)
            {
                productDb.Title = product.Title;
                productDb.Description = product.Description;
                productDb.Duration = product.Duration;
                productDb.YearOfRelease = product.YearOfRelease;
                productDb.ListPrice = product.ListPrice;
                productDb.Price = product.Price;
                productDb.PriceMoreThan3 = product.PriceMoreThan3;
                productDb.PriceMoreThan10 = product.PriceMoreThan10;
                productDb.CategoryId = product.CategoryId;
                productDb.ProductImages = product.ProductImages;
            }
            await _context.SaveChangesAsync();
        }
    }
}
