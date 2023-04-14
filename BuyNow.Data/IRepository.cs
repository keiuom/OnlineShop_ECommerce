using BuyNow.Core;
using System.Linq.Expressions;

namespace BuyNow.Data
{
    public interface IRepository<TEntity, TKey>
       where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity);

        Task RemoveAsync(TKey id);

        void Remove(TEntity entityToDelete);

        void Edit(TEntity entityToUpdate);

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null);

        Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);

        Task<IList<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(TKey id);
    }
}
