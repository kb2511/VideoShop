using WebShopModels;

namespace WebShopData.Interfaces
{
    public interface IProductImageService : IService<ProductImage>
    {
        Task Update(ProductImage image);
    }
}
