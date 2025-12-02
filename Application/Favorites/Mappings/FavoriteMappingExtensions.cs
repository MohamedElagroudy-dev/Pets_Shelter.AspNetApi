using Core.Entities.Product;
using Ecom.Application.Products.DTOs;
using Ecom.Application.Products.Mappings;
using Application.Favorites.DTOs;

namespace Application.Favorites.Mappings
{
    public static class FavoriteMappingExtensions
    {
        public static FavoriteProductDto ToFavoriteDto(this Product product, DateTime dateAdded)
        {
            var productDto = product.ToDto();

            return new FavoriteProductDto
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Photos = productDto.Photos,
                CategoryName = productDto.CategoryName,
                PetTypeName = productDto.PetTypeName,
                rating = productDto.rating,
                DateAdded = dateAdded
            };
        }
    }
}
