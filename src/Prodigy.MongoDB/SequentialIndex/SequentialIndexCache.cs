using System.Collections.Concurrent;

namespace Prodigy.MongoDB.SequentialIndex
{
    public class SequentialIndexCache : ISequentialIndexCache
    {
        private readonly ConcurrentDictionary<string, long> _cache;

        public SequentialIndexCache()
        {
            _cache = new ConcurrentDictionary<string, long>();
        }

        public long GetIndex(string collectionName)
        {
            if (_cache.TryGetValue(collectionName, out long index))
                return index;
            return -1;
        }

        public void AddIndex(string collectionName, long index)
        {
            _cache.TryAdd(collectionName, index);
        }

        public long UpdateIndex(string collectionName, long newIndex, long previousIndex)
        {
            if (_cache.TryGetValue(collectionName, out long _))
            {
                if (_cache.TryUpdate(collectionName, newIndex, previousIndex))
                    return newIndex;
            }

            return -1;
        }

        public void AddOrUpdateIndex(string collectionName, long index)
        {
            _cache.AddOrUpdate(collectionName, index, (c, i) => UpdateIndex(collectionName, index, i));
        }
    }
}
