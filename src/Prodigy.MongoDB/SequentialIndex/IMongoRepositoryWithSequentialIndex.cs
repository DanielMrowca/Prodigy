using Prodigy.Types;

namespace Prodigy.MongoDB.SequentialIndex
{
    public interface IMongoRepositoryWithSequentialIndex<TEntity,TKey> : IMongoRepository<TEntity,TKey>
        where TEntity : IIdentifiable<TKey>, ISequentialIndex<TKey>
    {
    }
}
