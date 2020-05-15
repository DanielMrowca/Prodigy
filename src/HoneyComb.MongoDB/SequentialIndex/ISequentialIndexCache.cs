using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MongoDB
{
    public interface ISequentialIndexCache
    {
        public long GetIndex(string collectionName);
        public void AddOrUpdateIndex(string collectionName, long index);
    }
}
