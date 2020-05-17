using HoneyComb.CQRS.Queries;
using HoneyComb.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB
{
    public static class Pagination
    {
        public static async Task<PagedResult<T>> PaginateAsync<T, TKey>(this IMongoQueryable<T> collection, int resultsPerPage = 10)
            where T : IIdentifiable<TKey>
        {
            if (resultsPerPage <= 0)
                resultsPerPage = 10;

            var result = await collection
              .Take(resultsPerPage + 1)
              .ToListAsync();

            var hasNextData = result.Count > resultsPerPage;
            if (result.Count > 1 && hasNextData)
                result.RemoveAt(result.Count - 1);
            var last = result.LastOrDefault();
            return PagedResult<T>.Create(result, result.Count, last?.Id.ToString(), hasNextData);
        }

       
        public static Task<PagedResult<T>> PaginateAsync<T, TKey>(this IMongoQueryable<T> collection, IPagedQuery query)
            where T : IIdentifiable<TKey>
            => PaginateAsync<T, TKey>(collection, query.Results);


        //public static async Task<PagedResult<TDocument>> PaginateAsync<TDocument>(
        //    this IMongoCollection<TDocument> collection,
        //    IPagedQuery query,
        //    FilterDefinition<TDocument> filterDefinition = null,
        //    SortDefinition<TDocument> sortDefinition = null,
        //    AggregateOptions options = null)
        //    => await collection.PaginateAsync(filterDefinition, sortDefinition, options, query.Page, query.Results);

        //public static async Task<PagedResult<TDocument>> PaginateAsync<TDocument>(
        //    this IMongoCollection<TDocument> collection,
        //    FilterDefinition<TDocument> filterDefinition,
        //    SortDefinition<TDocument> sortDefinition,
        //    AggregateOptions options = null,
        //    int page = 1, int resultsPerPage = 10)
        //{
        //    if (page <= 0)
        //        page = 1;

        //    if (resultsPerPage <= 0)
        //        resultsPerPage = 10;

        //    if (filterDefinition is null)
        //        filterDefinition = Builders<TDocument>.Filter.Empty;

        //    var countFacet = AggregateFacet.Create("count",
        //        PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
        //        {
        //            PipelineStageDefinitionBuilder.Count<TDocument>()
        //        }));

        //    var dataFacet = AggregateFacet.Create("data",
        //        PipelineDefinition<TDocument, TDocument>.Create(new[]
        //        {
        //            PipelineStageDefinitionBuilder.Sort(sortDefinition),
        //            PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * resultsPerPage),
        //            PipelineStageDefinitionBuilder.Limit<TDocument>(resultsPerPage)
        //        }));



        //    var aggregation = collection.Aggregate(options)
        //         .Match(filterDefinition)
        //         .Facet(countFacet, dataFacet);

        //    var aggregateResult = await aggregation.ToListAsync();



        //    var count = aggregateResult.First()
        //    .Facets.First(x => x.Name == "count")
        //    .Output<AggregateCountResult>()
        //    .First()
        //    .Count;

        //    var totalPages = (int)Math.Ceiling((double)count / resultsPerPage);
        //    var result = aggregateResult.First()
        //        .Facets.First(x => x.Name == "data")
        //        .Output<TDocument>();

        //    return PagedResult<TDocument>.Create(result, page, resultsPerPage, totalPages, count);
        //}


        //public static async Task<PagedResult<T>> PaginateAsync<T>(this IMongoQueryable<T> collection, IPagedQuery query)
        //    => await collection.PaginateAsync(query.Page, query.Results);

        //public static async Task<PagedResult<T>> PaginateAsync<T>(this IMongoQueryable<T> collection,
        //    int page = 1, int resultsPerPage = 10)
        //{
        //    if (page <= 0)
        //    {
        //        page = 1;
        //    }
        //    if (resultsPerPage <= 0)
        //    {
        //        resultsPerPage = 10;
        //    }
        //    var isEmpty = !await collection.AnyAsync();
        //    if (isEmpty)
        //    {
        //        return PagedResult<T>.Empty;
        //    }
        //    var totalResults = await collection.CountAsync();
        //    var totalPages = (int)Math.Ceiling((decimal)totalResults / resultsPerPage);
        //    if (page > totalPages)
        //        page = totalPages;

        //    var result = collection.Limit(page, resultsPerPage);
        //    //var executionModel = result.GetExecutionModel();
        //    var data = await result.ToListAsync();
        //    //return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        //    return PagedResult<T>.Create(data, page, resultsPerPage, totalPages, totalResults);
        //}

        //public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> collection, IPagedQuery query)
        //    => collection.Limit(query.Page, query.Results);

        //public static IMongoQueryable<T> Limit<T>(this IMongoQueryable<T> collection,
        //    int page = 1, int resultsPerPage = 10)
        //{
        //    if (page <= 0)
        //    {
        //        page = 1;
        //    }
        //    if (resultsPerPage <= 0)
        //    {
        //        resultsPerPage = 10;
        //    }
        //    var skip = (page - 1) * resultsPerPage;
        //    var data = collection.Skip(skip)
        //        .Take(resultsPerPage);

        //    return data;
        //}
    }
}

