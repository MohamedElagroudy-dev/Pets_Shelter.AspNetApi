using Application.Common;
using Core.Sharing.Pagination;
using Ecom.Application.Products.DTOs;

namespace Ecom.Application.Products.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDTO>> GetAllAsync(ProductParams productParams);
        Task<ProductDTO?> AddAsync(AddProductDTO dto);
        Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO);
        Task<ProductDTO?> DeleteAsync(int id);
        Task<ProductDTO?> GetProductAsync(int id);
    }
}
