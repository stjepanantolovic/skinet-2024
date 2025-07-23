using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.RequestHelpers
{
    public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
    {
        public int PageSize { get; set; } = pageSize > count ? count : pageSize;
        public int PageIndex { get; set; } = pageIndex;

        public int Count { get; set; } = count;
        public IReadOnlyList<T> Data { get; set; } = data;

        public int SetPageIndex()
        {
            var skip = pageSize * (pageIndex - 1);            
            var actualSkip = skip < count ? 0 : skip; 
            return actualSkip / PageSize + 1;
        }
    }
}