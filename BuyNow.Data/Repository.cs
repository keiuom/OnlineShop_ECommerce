using BuyNow.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BuyNow.Data
{
    public abstract class Repository<TEntity, TKey, TContext>
        : IRepository<TEntity, TKey>
         where TEntity : BaseEntity<TKey>
         where TContext : DbContext
    {
        protected TContext _dbContext;
        protected DbSet<TEntity> _dbSet;

        public Repository(TContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async virtual Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task RemoveAsync(TKey id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);

            if (entityToDelete != null)
                Remove(entityToDelete);
        }

        public virtual void Remove(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Edit(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void EditRange(IList<TEntity> entitiesToUpdate)
        {
            _dbSet.AttachRange(entitiesToUpdate);
            _dbSet.UpdateRange(entitiesToUpdate);
        }

        public virtual Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.CountAsync();
        }

        public async virtual Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async virtual Task<IList<TEntity>> GetAllAsync()
        {
            IQueryable<TEntity> query = _dbSet;
            return await query.ToListAsync();
        }

        public async virtual Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
