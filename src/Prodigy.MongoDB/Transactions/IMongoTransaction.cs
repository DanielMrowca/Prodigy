using System;
using System.Threading.Tasks;
using Prodigy.Repositories;

namespace Prodigy.MongoDB.Transactions
{
    public interface IMongoTransaction : ITransaction
    {
        void AddTransactionCommand(Func<Task> mongoCommand);
    }
}
