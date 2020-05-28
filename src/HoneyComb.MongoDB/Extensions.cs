using HoneyComb.MongoDB.Initializers;
using HoneyComb.MongoDB.Repositories;
using HoneyComb.Types;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

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

            builder.Services.AddTransient(sp =>
            {
                var options = sp.GetService<MongoDbOptions>();
                var client = sp.GetService<IMongoClient>();
                return client.GetDatabase(options.Database);
            });

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
                builder.Services.AddSingleton(sp =>
                {
                    var db = sp.GetService<IMongoDatabase>();
                    return new MongoIndexesInitializer<TEntity, TKey>(db, collectionName, indexes);
                });

                builder.AddInitializer<MongoIndexesInitializer<TEntity, TKey>>();
            }

            builder.Services.AddTransient<IMongoRepository<TEntity, TKey>>(sp =>
            {
                var db = sp.GetService<IMongoDatabase>();
                return new MongoRepository<TEntity, TKey>(db, collectionName);
            });

            return builder;
        }

        public static IHoneyCombBuilder AddMongoRepositoryWithSequentialIndex<TDocument, TKey>(this IHoneyCombBuilder builder, string collectionName,
            Action<List<CreateIndexModel<TDocument>>, IndexKeysDefinitionBuilder<TDocument>> indexBuilder = null)
           where TDocument : ISequentialIndex<TKey>
        {
            builder.AddMongoRepository<TDocument, TKey>(collectionName, indexBuilder);
            builder.Services.Decorate<IMongoRepository<TDocument, TKey>, MongoRepositoryWithSequentialIndexDecorator<TDocument, TKey>>();
            builder.Services.AddSingleton<ISequentialIndexCache>(new SequentialIndexCache());
            builder.Services.AddSingleton<ISequentialIndexProvider<TDocument, TKey>>(sp =>
            {
                var db = sp.GetService<IMongoDatabase>();
                var cache = sp.GetService<ISequentialIndexCache>();
                return new SequentialIndexProvider<TDocument, TKey>(db, collectionName, cache);
            });

            builder.AddInitializer<ISequentialIndexProvider<TDocument, TKey>>();

            return builder;
        }

    }
}
