using System.Linq.Expressions;
using WebShopData.Data;
using WebShopData.Interfaces;
using WebShopModels;

namespace WebShopData.Services
{
    public class CategoryService : Service<Category>, ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context) 
            :base(context)
        {
            _context = context;
        }

        public async Task Update(Category category)
        {
            _context.Category.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
