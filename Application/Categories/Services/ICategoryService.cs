using Application.Categories.DTOs;

namespace Application.Categories.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO> GetProductAsync(int id);
        Task<CategoryDTO> AddAsync(AddCategoryDTO dto);
        Task<CategoryDTO> UpdateAsync(UpdateCategoryDTO updateDTO);
        Task DeleteAsync(int id);
        Task<bool> CategoryExistsAsync(int id);

    }
}
