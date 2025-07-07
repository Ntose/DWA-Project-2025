using System.Collections.Generic;

namespace WebAPI.Infrastructure
{
    /// <summary>
    /// Generic container for paginated API responses.
    /// </summary>
    /// <typeparam name="T">The type of items being paged.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// The items on the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
