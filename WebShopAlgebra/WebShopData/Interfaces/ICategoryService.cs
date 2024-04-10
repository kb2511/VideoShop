using WebShopModels;

namespace WebShopData.Interfaces
{
    public interface ICategoryService : IService<Category>
    {
        Task Update(Category category);
    }
}
