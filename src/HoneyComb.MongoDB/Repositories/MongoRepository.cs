using HoneyComb.CQRS.Queries;
using HoneyComb.Types;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HoneyComb.MongoDB;
using MongoDB.Driver.Linq;
using System.Linq;
using MongoDB.Driver.Builders;

namespace HoneyComb.MongoDB.Repositories
{
    internal class MongoRepository<TEntity, TKey> : IMongoRepository<TEntity, TKey>
        where TEntity : IIdentifiable<TKey>
    {
        public IMongoCollection<TEntity> Collection { get; }

        public MongoRepository(IMongoDatabase database, string collectionName, string[] indexes = null)
        {
            Collection = database.GetCollection<TEntity>(collectionName);
            if (indexes != null && indexes.Count() > 0)
            {
                foreach (string index in indexes)
                {
                    var keys = Builders<TEntity>.IndexKeys.Ascending(index);
                    var options = new CreateIndexOptions() { Unique = true };
                    var model = new CreateIndexModel<TEntity>(keys, options);
                    Collection.Indexes.CreateOneAsync(model);
                }
            }          
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
            => await Collection.AsQueryable().Where(predicate).PaginateAsync(query);


        public async Task AddAsync(TEntity entity)
            => await Collection.InsertOneAsync(entity);

        public async Task UpdateAsync(TEntity entity)
            => await Collection.ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);

        public async Task DeleteAsync(TKey id)
            => await Collection.DeleteOneAsync(x => x.Id.Equals(id));

        public async Task AddMultipleAsync(IEnumerable<TEntity> entities)
            => await Collection.InsertManyAsync(entities);
    }
}
