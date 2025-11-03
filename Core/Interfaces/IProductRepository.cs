using Core.Constants;
using Core.Entities.Product;
using Core.Sharing;
using Core.Sharing.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            int? categoryId,
            int? petTypeId,
            ProductSort? sort);

    }
}
