using WebShopModels;

namespace WebShopData.Interfaces
{
    public interface IShoppingCartService : IService<ShoppingCart>
    {
        Task Update(ShoppingCart shoppingCart);
    }
}
