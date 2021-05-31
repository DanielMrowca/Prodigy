using HoneyComb.MongoDB.Contexts;
using HoneyComb.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Transactions
{
    public class MongoTransaction : IMongoTransaction, ITransaction
    {
        // Every command will be stored here and it'll be processed at CommitTransactionAsync
        private readonly List<Func<Task>> _commands = new List<Func<Task>>();
        private readonly IMongoClient _mongoClient;

        public bool IsInTransaction { get; private set; }

        public MongoTransaction(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            IsInTransaction = true;
        }

        public void AddTransactionCommand(Func<Task> mongoCommand)
        {
            _commands.Add(mongoCommand);
        }

        public async Task<bool> CommitTransactionAsync()
        {
            using (var session = await _mongoClient.StartSessionAsync())
            {
                session.StartTransaction();
                var commandTasks = _commands.Select(c => c());
                await Task.WhenAll(commandTasks);
                await session.CommitTransactionAsync();
                _commands.Clear();
                IsInTransaction = false;
            }

            
            return _commands.Count > 0;
        }

        public Task AbortTransactionAsync()
        {
            _commands.Clear();
            IsInTransaction = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _commands.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
