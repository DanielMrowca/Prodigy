using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Prodigy.CQRS.Queries;
using Prodigy.Types;

namespace Prodigy.MongoDB
{
    public interface IMongoRepository<TEntity, in TKey> where TEntity : IIdentifiable<TKey>
    {
        IMongoCollection<TEntity> Collection { get; }
        Task<TEntity> GetAsync(TKey id);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
            TQuery query) where TQuery : IPagedQuery;

        Task<TEntity> AddAsync(TEntity entity);
        Task<IList<TEntity>> AddMultipleAsync(IList<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
