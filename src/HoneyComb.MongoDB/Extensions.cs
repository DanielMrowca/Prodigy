using HoneyComb.MongoDB.Repositories;
using HoneyComb.Types;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static IHoneyCombBuilder AddMongoRepository<TEntity,TKey>(this IHoneyCombBuilder builder, string collectionName, string[] index = null)
            where TEntity : IIdentifiable<TKey>
        {
            builder.Services.AddTransient<IMongoRepository<TEntity, TKey>>(sp =>
            {
                var db = sp.GetService<IMongoDatabase>();
                return new MongoRepository<TEntity, TKey>(db, collectionName, index);
            });

            return builder;
        }
    }
}
