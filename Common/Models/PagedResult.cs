using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class PagedResult<T>
    {
        public PagedResult(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize, bool succeeded = true, string message = null, IEnumerable<string> errors = null)
        {
            Items = items;
            Data = items; // Compatibility alias
            TotalCount = count;
            PageNumber = pageNumber;
            CurrentPage = pageNumber; // Compatibility alias
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            HasPreviousPage = pageNumber > 1;
            HasNextPage = pageNumber < TotalPages;
            Succeeded = succeeded;
            Message = message;
            Errors = errors?.ToList() ?? new List<string>();
        }

        public IReadOnlyCollection<T> Items { get; }
        public IReadOnlyCollection<T> Data { get; } // Compatibility alias
        public int PageNumber { get; }
        public int CurrentPage { get; } // Compatibility alias
        public int PageSize { get; set; } // Made settable for compatibility
        public int TotalPages { get; set; } // Made settable for compatibility
        public int TotalCount { get; set; } // Made settable for compatibility
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }

        // Added missing properties for error handling
        public bool Succeeded { get; }
        public string Message { get; }
        public IList<string> Errors { get; }

        public static PagedResult<T> Create(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(items, count, pageNumber, pageSize, true);
        }

        // Additional factory methods for error scenarios
        public static PagedResult<T> Failure(string message, IEnumerable<string> errors = null)
        {
            return new PagedResult<T>(new List<T>(), 0, 1, 10, false, message, errors);
        }

        public static PagedResult<T> Success(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize, string message = null)
        {
            return new PagedResult<T>(items, count, pageNumber, pageSize, true, message);
        }
    }
}