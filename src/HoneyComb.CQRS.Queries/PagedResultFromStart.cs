using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public class PagedResultFromStart<T> : PagedResultFromStartBase
    {
        public IEnumerable<T> Items { get; }

        public bool IsEmpty => Items is null || !Items.Any();
        public bool IsNotEmpty => !IsEmpty;

        protected PagedResultFromStart()
        {
            Items = Enumerable.Empty<T>();
        }

        [JsonConstructor]
        protected PagedResultFromStart(IEnumerable<T> items,
      int currentPage, int resultsPerPage,
      int totalPages, long? totalResults) :
          base(currentPage, resultsPerPage, totalPages, totalResults)
        {
            Items = items;
        }

        public static PagedResultFromStart<T> Create(IEnumerable<T> items,
            int currentPage, int resultsPerPage,
            int totalPages, long totalResults)
            => new PagedResultFromStart<T>(items, currentPage, resultsPerPage, totalPages, totalResults);

        public static PagedResultFromStart<T> From(PagedResultFromStartBase result, IEnumerable<T> items)
            => new PagedResultFromStart<T>(items, result.CurrentPage, result.ResultsPerPage,
                result.TotalPages, result.TotalResults);

        public static PagedResultFromStart<T> Empty => new PagedResultFromStart<T>();

        public PagedResultFromStart<U> Map<U>(Func<T, U> map)
            => PagedResultFromStart<U>.From(this, Items.Select(map));
    }
}
