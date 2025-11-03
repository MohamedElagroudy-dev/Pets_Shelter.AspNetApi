using Core.Entities.Product;
using Ecom.Application.Products.DTOs;
using Ecom.Core.Entities.Product;

namespace Ecom.Application.Products.Mappings
{
    public static class ProductMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/42463.jpg";

        public static ProductDTO ToDto(this Product product)
        {
            var photos = product.Photos?.Select(p => p.ToDto()).ToList() ?? new List<PhotoDTO>();

            // If there are no photos, add a default one
            if (photos.Count == 0)
            {
                photos.Add(new PhotoDTO
                {
                    ImageName = DefaultImagePath,
                    ProductId = product.Id
                });
            }

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryName = product.Category?.Name ?? string.Empty,
                PetTypeName = product.PetType?.Name ?? string.Empty,
                Photos = photos,
                rating = product.rating
            };
        }

        public static PhotoDTO ToDto(this Photo photo)
        {
            return new PhotoDTO
            {
                ImageName = photo.ImageName,
                ProductId = photo.ProductId
            };
        }

        public static Product ToEntity(this AddProductDTO dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                CategoryId = dto.CategoryId,
                PetTypeId = dto.PetTypeId,
                Photos = new List<Photo>()
            };
        }

        public static void UpdateEntity(this Product product, UpdateProductDTO dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.QuantityInStock = dto.QuantityInStock;
            product.CategoryId = dto.CategoryId;
            product.PetTypeId = dto.PetTypeId;
        }
    }
}
