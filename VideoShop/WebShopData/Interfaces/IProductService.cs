using WebShopModels;

namespace WebShopData.Interfaces
{
    public interface IProductService : IService<Product>
    {
        Task Update(Product product);
    }
}
