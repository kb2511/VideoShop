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
                productDb.PriceMoreThen3 = product.PriceMoreThen3;
                productDb.PriceMoreThen10 = product.PriceMoreThen10;
                productDb.CategoryId = product.CategoryId;

                if(product.ImageUrl != null)
                {
                    productDb.ImageUrl = product.ImageUrl;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
