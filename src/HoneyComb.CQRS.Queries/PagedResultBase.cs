using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public abstract class PagedResultBase
    {
        public int ResultsPerPage { get; }
        public string LastId { get; }
        public bool HasNextData { get; }

        //public int CurrentPage { get; }
        //public int ResultsPerPage { get; }
        //public int TotalPages { get; }
        public long? TotalResults { get; }

        protected PagedResultBase()
        {
        }

        protected PagedResultBase(int resultsPerPage, string lastId,
            bool hasNextData, long? totalResults = null)
        {
            ResultsPerPage = resultsPerPage;
            LastId = lastId;
            HasNextData = hasNextData;
            TotalResults = totalResults;
        }

        //protected PagedResultBase(int currentPage, int resultsPerPage,
        //    int totalPages, long totalResults)
        //{
        //    CurrentPage = currentPage > totalPages ? totalPages : currentPage;
        //    ResultsPerPage = resultsPerPage;
        //    TotalPages = totalPages;
        //    TotalResults = totalResults;
        //}
    }
}
