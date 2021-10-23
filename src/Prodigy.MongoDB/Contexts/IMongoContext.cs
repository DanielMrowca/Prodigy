using MongoDB.Driver;
using Prodigy.MongoDB.Transactions;
using Prodigy.Repositories;

namespace Prodigy.MongoDB.Contexts
{
    public interface IMongoContext : ITransactionScope
    {
        bool IsActiveTransaction { get; }
        IMongoTransaction Transaction { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
