using HoneyComb.MongoDB.Contexts;
using HoneyComb.MongoDB.Initializers;
using HoneyComb.MongoDB.Repositories;
using HoneyComb.MongoDB.Transactions;
using HoneyComb.Repositories;
using HoneyComb.Types;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace HoneyComb.MongoDB
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddMongo(this IHoneyCombBuilder builder, MongoDbOptions dbOptions)
        {
            builder.Services.AddSingleton(dbOptions);
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var options = sp.GetService<MongoDbOptions>();
                return new MongoClient(options.ConnectionString);
            });

            builder.Services.AddScoped(sp =>
            {
                var options = sp.GetService<MongoDbOptions>();
                var client = sp.GetService<IMongoClient>();
                return client.GetDatabase(options.Database);
            });

            // Required for transactional mongo repository
            builder.AddMongoContext();

            return builder;
        }

        public static IHoneyCombBuilder AddMongo(this IHoneyCombBuilder builder, string settingsSection)
        {
            var options = builder.GetSettings<MongoDbOptions>(settingsSection);
            return builder.AddMongo(options);
        }

        public static IHoneyCombBuilder AddMongoRepository<TEntity, TKey>(this IHoneyCombBuilder builder, string collectionName,
            Action<List<CreateIndexModel<TEntity>>, IndexKeysDefinitionBuilder<TEntity>> indexBuilder = null)
            where TEntity : IIdentifiable<TKey>
        {

            var indexes = new List<CreateIndexModel<TEntity>>();
            var indexKeyBuilder = Builders<TEntity>.IndexKeys;
            indexBuilder?.Invoke(indexes, indexKeyBuilder);
            if (indexes.Count > 0)
            {
                builder.Services.AddSingleton<IInitializer>(sp =>
                {
                    var db = sp.CreateScope().ServiceProvider.GetService<IMongoDatabase>();
                    return new MongoIndexesInitializer<TEntity, TKey>(db, collectionName, indexes);
                });
            }

            builder.Services.AddScoped<IMongoRepository<TEntity, TKey>>(sp =>
            {
                var context = sp.GetService<IMongoContext>();
                return new MongoRepository<TEntity, TKey>(context, collectionName);
            });

            return builder;
        }

        public static IHoneyCombBuilder AddMongoRepositoryWithSequentialIndex<TDocument, TKey>(this IHoneyCombBuilder builder, string collectionName,
            Action<List<CreateIndexModel<TDocument>>, IndexKeysDefinitionBuilder<TDocument>> indexBuilder = null)
           where TDocument : ISequentialIndex<TKey>
        {
            builder.AddMongoRepository<TDocument, TKey>(collectionName, indexBuilder);
            builder.Services.Decorate<IMongoRepository<TDocument, TKey>, MongoRepositoryWithSequentialIndexDecorator<TDocument, TKey>>();
            builder.Services.AddSingleton<ISequentialIndexCache, SequentialIndexCache>();
            builder.Services.AddScoped<ISequentialIndexProvider<TDocument, TKey>>(sp =>
            {
                var db = sp.GetService<IMongoContext>();
                var cache = sp.GetService<ISequentialIndexCache>();
                return new SequentialIndexProvider<TDocument, TKey>(db, collectionName, cache);
            });

            builder.Services.AddSingleton<IInitializer>(sp => sp.GetService<ISequentialIndexProvider<TDocument, TKey>>());
            return builder;
        }

        private static void AddMongoContext(this IHoneyCombBuilder builder)
        {
            var sp = builder.Services.BuildServiceProvider();

            var mongoContext = new MongoTransactionalContext(sp.GetRequiredService<IMongoDatabase>(), sp.GetRequiredService<IMongoClient>());

            builder.Services.AddScoped<IMongoContext>(sp => mongoContext);
            builder.Services.AddScoped<ITransactionScope>(sp => mongoContext);
        }
    }
}
