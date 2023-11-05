using Microsoft.EntityFrameworkCore;
using Repositories.Concrete.EfCore;
using Repositories.Contracts;
using System.Linq.Expressions;

namespace Repositories.Concrete
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly RepositoryContext context;

        protected RepositoryBase(RepositoryContext context)
        {
            this.context = context;
        }

        public IQueryable<TEntity> FindAll(bool trackChanges = false) =>
            !trackChanges ? context.Set<TEntity>().AsNoTracking() : context.Set<TEntity>();

        public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression,
            bool trackChanges = false) =>
            !trackChanges
                ? context.Set<TEntity>().Where(expression).AsNoTracking()
                : context.Set<TEntity>().Where(expression);

        public void Create(TEntity entity) => context.Set<TEntity>().Add(entity);

        public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity) => context.Set<TEntity>().Remove(entity);
    }
}
