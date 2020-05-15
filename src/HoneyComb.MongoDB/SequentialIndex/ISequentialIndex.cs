using HoneyComb.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MongoDB
{
    public interface ISequentialIndex<TKey> : IIdentifiable<TKey>
    {
        public long Index { get; set; }
    }
}
