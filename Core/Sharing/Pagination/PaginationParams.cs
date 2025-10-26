using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Pagination
{
    public abstract class PaginationParams
    {
        private const int MaxPageSizeDefault = 10;
        private int _pageSize = 3;

        public int PageNumber { get; set; } = 1;

        private int _maxPageSize = MaxPageSizeDefault;
        public int MaxPageSize
        {
            get => _maxPageSize;
            set => _maxPageSize = value > 0 ? value : MaxPageSizeDefault;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Search { get; set; }
    }

}
