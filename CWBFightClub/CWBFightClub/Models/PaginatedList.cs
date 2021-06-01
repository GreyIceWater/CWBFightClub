using System;
using System.Collections.Generic;
using System.Linq;

namespace CWBFightClub.Models
{
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// The current page.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Total pages available.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Value indicating if previous page exists.
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        /// <summary>
        /// Value indicating if next page exists.
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PaginatedList class.
        /// </summary>
        /// <param name="items">The list to paginate.</param>
        /// <param name="count">The number of items in the list.</param>
        /// <param name="pageIndex">The current page marker.</param>
        /// <param name="pageSize">The number of items per page.</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            int possibleTotalPages = (int)Math.Ceiling((double)count / (double)pageSize);
            this.TotalPages = possibleTotalPages == 0 ? 1 : possibleTotalPages;
            this.AddRange(items);
        }

        /// <summary>
        /// Create a paginated list from a source.
        /// </summary>
        /// <param name="source">The source to turn into a paginated list.</param>
        /// <param name="pageIndex">The current page marker.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns></returns>
        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = source.Count();
            List<T> items = source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
