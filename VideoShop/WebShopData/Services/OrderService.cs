using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopModels;

namespace WebShopData.Services
{
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context) 
            : base(context)
        {
            _context = context;
        }

        public async Task Update(Order order)
        {
            _context.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
