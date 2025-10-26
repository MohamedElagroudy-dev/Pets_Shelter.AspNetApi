using Application.Categories.DTOs;
using Application.Categories.Mappings;
using Core.Entities.Product;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CategoryDTO>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all categories...");
            var categories = await _unitOfWork.Categories.GetAllAsync();

            if (categories == null || !categories.Any())
                throw new KeyNotFoundException("No categories found");

            return categories.Select(c => c.ToDto()).ToList();
        }

        public async Task<CategoryDTO> AddAsync(AddCategoryDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Category name cannot be empty");

            var category = dto.ToEntity();

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return category.ToDto();
        }

        public async Task<CategoryDTO> UpdateAsync(UpdateCategoryDTO updateDTO)
        {
            var existing = await _unitOfWork.Categories.GetAsync(updateDTO.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Category with Id={updateDTO.Id} not found");

            existing.UpdateEntity(updateDTO);

            await _unitOfWork.Categories.UpdateAsync(existing.Id, existing);
            await _unitOfWork.CompleteAsync();

            return existing.ToDto();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Categories.GetAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Category with Id={id} not found");

            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<CategoryDTO> GetProductAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with Id={id} not found");

            return category.ToDto();
        }
        public async Task<bool> CategoryExistsAsync(int id)
        {
            _logger.LogInformation("Checking if category with Id {Id} exists...", id);

            var exists = await _unitOfWork.Categories.GetAsync(id);
            return exists != null;
        }

    }
}
