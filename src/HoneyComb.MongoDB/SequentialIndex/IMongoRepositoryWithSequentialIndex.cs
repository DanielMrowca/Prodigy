using HoneyComb.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MongoDB
{
    public interface IMongoRepositoryWithSequentialIndex<TEntity,TKey> : IMongoRepository<TEntity,TKey>
        where TEntity : IIdentifiable<TKey>, ISequentialIndex<TKey>
    {
    }
}
