using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        /// <summary>
        /// Asynchronously creates sub set(collection) of records based on page criteria.  
        /// </summary>
        /// <param name="source">IQueryable collection of records</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged list of records</returns>
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // TODO: Check when the query to database will be executed, as the source is of type IQueryable.
            // PLINQ: CountAsync - Asynchronously returns the number of elements in a sequence.
            var count = await source.CountAsync();
            // LINQ: Skip - Bypasses a specified number of elements in a sequence and then returns the remaining elements.
            // LINQ: Take - Returns a specified number of contiguous elements from the start of a sequence.
            // PLINQ: ToListAsync - Asynchronously creates a System.Collections.Generic.List1 from an System.Linq.IQueryable1 by enumerating it asynchronously.
            var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync();
            
            return new PagedList<T>(items, count, pageNumber, pageSize); 
        }
    }
}