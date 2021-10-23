namespace Prodigy.MongoDB.SequentialIndex
{
    public interface ISequentialIndexCache
    {
        public long GetIndex(string collectionName);
        public void AddOrUpdateIndex(string collectionName, long index);
    }
}
