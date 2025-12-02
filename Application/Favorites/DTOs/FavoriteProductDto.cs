using Ecom.Application.Products.DTOs;
using System;

namespace Application.Favorites.DTOs
{
    public record FavoriteProductDto : ProductDTO
    {
        public DateTime DateAdded { get; init; }
    }
}
