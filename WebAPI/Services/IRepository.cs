using System.Linq.Expressions;

namespace WebAPI.Services
{
    public interface IRepository<TEntity, TResult> where TEntity : class
    {
        TEntity Get(int id);

        IList<TEntity> GetAll();
        IList<TEntity> Get<TParameter>(Expression<Func<TEntity, TParameter>> incluedProperty);
        IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate, List<string> includes = null);
        IList<TResult> FindWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, List<string> includes = null);

        void Add(TEntity entity);
        void AddRange(IList<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IList<TEntity> entities);
        void Update(TEntity entity);
    }
}
