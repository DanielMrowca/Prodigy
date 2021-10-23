namespace Prodigy.Types
{
    public interface IIdentifiable<out TKey>
    {
        TKey Id { get; }
    }
}
