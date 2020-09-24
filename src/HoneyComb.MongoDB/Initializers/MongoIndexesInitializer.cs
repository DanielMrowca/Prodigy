using HoneyComb.Types;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
            var currentIndexes = (_collection.Indexes.List().ToList()).Select(x => JsonConvert.DeserializeObject<dynamic>(x.ToString()));
            foreach (var newIndex in _indexes)
            {
                try
                {
                    await _collection.Indexes.CreateOneAsync(newIndex);
                }
                catch (MongoCommandException ex)
                {
                    // Check if message applies to specified case
                    if (ex.Code == 85 && ex.CodeName == "IndexOptionsConflict")
                    {
                        var messagePattern = "already exists with different options:";
                        var indexName = newIndex.Options.Name;
                        var existingIndex = currentIndexes.SingleOrDefault(x => x.name == indexName);
                        var existIndexWithAnotherName = existingIndex is null && ex.Message.Contains(messagePattern);
                        if (existIndexWithAnotherName) // Exists index with another name and for same field BUT HAVE ANOTHER OPTIONS
                        {
                            // ex.Message looks like below
                            // Command createIndexes failed: Index: { v: 2, key: { Date: -1 }, name: "New_Index_Name", expireAfterSeconds: 604800.0 } already exists with different options: { v: 2, key: { Date: -1 }, name: "Existing_Index_Name", background: false }.
                            // I'm parsing ex.Message to get name of existing Index (which name is another than the new one) in db because i want to drop this index and create new one

                            var similarIndexJson = ex.Message.Substring(ex.Message.IndexOf(messagePattern) + messagePattern.Length).Replace(".", "");
                            dynamic similarIndex = JsonConvert.DeserializeObject(similarIndexJson);
                            indexName = similarIndex.name.Value;
                            existingIndex = currentIndexes.SingleOrDefault(x => x.name == indexName);
                            if (existingIndex is null)
                                throw;
                        }

                        await _collection.Indexes.DropOneAsync(indexName);
                        await _collection.Indexes.CreateOneAsync(newIndex);
                    }
                    else
                        throw;

                }
            }
        }
    }
}
