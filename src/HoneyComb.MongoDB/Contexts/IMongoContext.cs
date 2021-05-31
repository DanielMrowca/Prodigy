using HoneyComb.MongoDB.Transactions;
using HoneyComb.Repositories;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Contexts
{
    public interface IMongoContext : ITransactionScope
    {
        bool IsActiveTransaction { get; }
        IMongoTransaction Transaction { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
