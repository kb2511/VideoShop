using WebShopModels;

namespace WebShopData.Interfaces
{
    public interface IOrderService : IService<Order>
    {
        Task Update(Order order);
    }
}
