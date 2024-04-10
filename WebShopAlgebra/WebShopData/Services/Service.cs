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

        public async Task<IEnumerable<T>> GetAll()
        {
            return await contextSet.ToListAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            return await contextSet.FirstOrDefaultAsync(filter);
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            contextSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
