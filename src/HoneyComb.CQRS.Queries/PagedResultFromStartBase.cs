using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public abstract class PagedResultFromStartBase
    {
        public int ResultsPerPage { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }
        public long? TotalResults { get; }

        protected PagedResultFromStartBase()
        {
        }

        protected PagedResultFromStartBase(int currentPage, int resultsPerPage,
            int totalPages, long? totalResults)
        {
            CurrentPage = currentPage > totalPages ? totalPages : currentPage;
            ResultsPerPage = resultsPerPage;
            TotalPages = totalPages;
            TotalResults = totalResults;
        }
    }
}
