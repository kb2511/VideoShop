using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebShopData.Data;
using WebShopData.Interfaces;

namespace WebShopData.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> contextSet;
        public Service(ApplicationDbContext context)
        {
            _context = context;
            this.contextSet = _context.Set<T>();
            //_context.Category == contextSet
            //_context.Product.Include(u => u.Category).Include(u => u.CategoryId);
        }
        public async Task Create(T entity)
        {
            contextSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            contextSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll(string[]? includeProperties = null)
        {
            IQueryable<T> query = contextSet;

            if (includeProperties?.Length != 0 && includeProperties != null)
            {
                foreach (var property in includeProperties.Where(p => p.Trim() != string.Empty))
                {
                    query = query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter, string[]? includeProperties = null)
        {
            IQueryable<T> query = contextSet;

            if (includeProperties?.Length != 0 && includeProperties != null)
            {
                foreach (var property in includeProperties.Where(p => p.Trim() != string.Empty))
                {
                    query = query.Include(property);
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            contextSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
