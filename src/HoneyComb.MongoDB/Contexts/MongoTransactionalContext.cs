using HoneyComb.MongoDB.Transactions;
using HoneyComb.Repositories;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Contexts
{
    public class MongoTransactionalContext : IMongoContext
    {
        
        private readonly IMongoDatabase _database;
        private readonly IMongoClient _mongoClient;

        public IMongoTransaction Transaction { get; private set; }
        public bool IsActiveTransaction => Transaction != null && Transaction.IsInTransaction;

        public MongoTransactionalContext(IMongoDatabase database, IMongoClient mongoClient)
        {
            _database = database;
            _mongoClient = mongoClient;
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public Task<ITransaction> StartTransactionAsync()
        {
            Transaction = new MongoTransaction(_mongoClient);
            return Task.FromResult((ITransaction)Transaction);
        }
    }
}
