using System.Linq.Expressions;

namespace WebShopData.Interfaces
{
    public interface IService<T> where T : class
    {
        //T - Category
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task Create(T entity);
        Task Delete(T entity);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
