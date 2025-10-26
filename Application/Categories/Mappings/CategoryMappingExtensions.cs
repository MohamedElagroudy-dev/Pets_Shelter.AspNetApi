using Application.Categories.DTOs;
using Core.Entities.Product;
using Ecom.Application.Products.DTOs;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Categories.Mappings
{
    public static class CategoryMappingExtensions
    {
        public static CategoryDTO ToDto(this Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
     
        public static Category ToEntity(this AddCategoryDTO dTO)
        {
            return new Category
            {
                Name = dTO.Name,
                Description = dTO.Description
            };
        }
        public static void UpdateEntity(this Category category, UpdateCategoryDTO dto)
        {
            category.Name = dto.Name;
            category.Description = dto.Description;
            
        }
    }
}
