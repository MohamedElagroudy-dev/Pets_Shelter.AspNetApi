using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sharing.Pagination
{
    public class ProductParams : PaginationParams
    {
        public ProductSort? Sort { get; set; } = ProductSort.Id;
        public int? CategoryId { get; set; }
        public int? TotalCount { get; set; }
    }
}
