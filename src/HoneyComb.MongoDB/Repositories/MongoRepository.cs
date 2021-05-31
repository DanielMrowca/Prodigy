using HoneyComb.CQRS.Queries;
using HoneyComb.Types;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Linq;
using HoneyComb.MongoDB.Contexts;

namespace HoneyComb.MongoDB.Repositories
{
    public class MongoRepository<TEntity, TKey> : IMongoRepository<TEntity, TKey>
        where TEntity : IIdentifiable<TKey>
    {
        private readonly IMongoContext _context;

        public IMongoCollection<TEntity> Collection { get; }

        public MongoRepository(IMongoContext context, string collectionName)
        {
            _context = context;
            Collection = context.GetCollection<TEntity>(collectionName);
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).ToListAsync();

        public async Task<TEntity> GetAsync(TKey id)
            => await GetAsync(x => x.Id.Equals(id));

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).SingleOrDefaultAsync();

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).AnyAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity,
            bool>> predicate, TQuery query) where TQuery : IPagedQuery
            => await Collection.AsQueryable().Where(predicate).PaginateAsync<TEntity, TKey>(query);


        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (_context.IsActiveTransaction)
                _context.Transaction.AddTransactionCommand(() => Collection.InsertOneAsync(entity));
            else
                await Collection.InsertOneAsync(entity);

            return entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (_context.IsActiveTransaction)
                _context.Transaction.AddTransactionCommand(() => Collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity));
            else
                await Collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);
        }
            

        public virtual async Task DeleteAsync(TKey id)
        {
            if (_context.IsActiveTransaction)
                _context.Transaction.AddTransactionCommand(() => Collection.DeleteOneAsync(x => x.Id.Equals(id)));
            else
                await Collection.DeleteOneAsync(x => x.Id.Equals(id));
        }
            

        public virtual async Task<IList<TEntity>> AddMultipleAsync(IList<TEntity> entities)
        {
            if (_context.IsActiveTransaction)
                _context.Transaction.AddTransactionCommand(() => Collection.InsertManyAsync(entities.ToList()));
            else
                await Collection.InsertManyAsync(entities.ToList());

            return entities;
        }
    }
}
