using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public interface IPagedQuery : IQuery
    {
        int Page { get; }
        int Results { get; }
        string OrderBy { get; }
        string SortOrder { get; }
        string LastId { get; }
        int Skip { get; }
    }
}
