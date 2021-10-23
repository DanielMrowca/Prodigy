using System.Threading.Tasks;
using Prodigy.Types;

namespace Prodigy.MongoDB.SequentialIndex
{
    public interface ISequentialIndexProvider<TDocument, TKey> : IInitializer
        where TDocument : ISequentialIndex<TKey>
    {
        public string CollectionName { get; }
        public long GetNextIndex();
        public void SetCurrentIndex(long index);
        public Task<long> GetLastIndexFromDb();
    }
}
