using System.Linq.Expressions;

namespace WebShopData.Interfaces
{
    public interface IService<T> where T : class
    {
        //T - Category
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string[]? includeProperties = null);
        Task<T> Get(Expression<Func<T, bool>> filter, string[]? includeProperties = null);
        Task Create(T entity);
        Task Delete(T entity);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
