using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Services;

namespace WebAPI.Models
{
    public class BaseRepository<TEntity, TResult> : IRepository<TEntity, TResult> where TEntity : class
    {
        internal DbContext context;
        internal DbSet<TEntity> dbSet;

        public BaseRepository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void AddRange(IList<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate, List<string> includes = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (predicate != null)
            {
                query = query.AsNoTracking().Where(predicate);
                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }
            }
            return query.ToList();
        }

        public IList<TResult> FindWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, List<string> includes = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (predicate != null)
            {
                query = query.AsNoTracking().Where(predicate);
                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = query.Include(include);
                    }
                }
            }

            return query.Select(selector).ToList();

        }

        public TEntity Get(int id)
        {
            return dbSet.Find(id);
        }

        public IList<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public IList<TEntity> Get<TParamater>(Expression<Func<TEntity, TParamater>> includeProperty)
        {
            dbSet.Include(includeProperty);
            return dbSet.ToList();
        }

        public void Remove(TEntity entity)
        {
            if (entity != null)
                Delete(entity);
        }

        public void RemoveRange(IList<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}