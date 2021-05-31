using HoneyComb.MongoDB.Contexts;
using HoneyComb.MongoDB.Repositories;
using HoneyComb.Types;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB
{
    public class SequentialIndexProvider<TDocument, TKey> : ISequentialIndexProvider<TDocument, TKey>, IInitializer
        where TDocument : ISequentialIndex<TKey>
    {
        static object _lock = new object();

        private readonly IMongoRepository<TDocument, TKey> _repository;
        private readonly ISequentialIndexCache _cache;
        public string CollectionName { get; private set; }

        public SequentialIndexProvider(IMongoContext mongoContext, string collectionName, ISequentialIndexCache cache)
        {
            CollectionName = collectionName;
            _repository = new MongoRepository<TDocument, TKey>(mongoContext, collectionName);
            _cache = cache;
        }



        public long GetNextIndex()
        {
            lock (_lock)
            {
                var index = _cache.GetIndex(CollectionName);
                if (index == -1)
                    index = 0;

                index++;
                _cache.AddOrUpdateIndex(CollectionName, index);
                return index;
            }
        }

        public async Task<long> GetLastIndexFromDb()
        {
            var result = await _repository.Collection
                          .AsQueryable()
                          .OrderByDescending(x => x.Index)
                          .FirstOrDefaultAsync();
            if (result is null)
                return -1;

            return result.Index;
        }

        public void SetCurrentIndex(long index)
        {
            lock (_lock)
            {
                _cache.AddOrUpdateIndex(CollectionName, index);
            }
        }

        public async Task InitializeAsync()
        {
            var index = await GetLastIndexFromDb();
            if (index == -1)
                SetCurrentIndex(0);
            else
                SetCurrentIndex(index);
        }
    }
}
