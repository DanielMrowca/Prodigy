using HoneyComb.Types;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Initializers
{
    public class MongoIndexesInitializer<TEntity, TKey> : IInitializer
        where TEntity : IIdentifiable<TKey>
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly IEnumerable<CreateIndexModel<TEntity>> _indexes;

        public MongoIndexesInitializer(IMongoDatabase database, string collectionName, IEnumerable<CreateIndexModel<TEntity>> indexes)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
            _indexes = indexes;
        }

        public async Task InitializeAsync()
        {
            await _collection.Indexes.CreateManyAsync(_indexes);
        }
    }
}
