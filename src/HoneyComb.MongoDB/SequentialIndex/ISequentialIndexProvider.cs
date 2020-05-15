using HoneyComb.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB
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
