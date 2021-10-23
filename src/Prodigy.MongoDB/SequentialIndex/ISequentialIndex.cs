using Prodigy.Types;

namespace Prodigy.MongoDB.SequentialIndex
{
    public interface ISequentialIndex<TKey> : IIdentifiable<TKey>
    {
        public long Index { get; set; }
    }
}
