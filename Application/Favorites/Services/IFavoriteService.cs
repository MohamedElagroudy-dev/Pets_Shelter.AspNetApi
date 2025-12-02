using Core.Entities.Product;
using Core.Entities;
using Application.Favorites.DTOs;

namespace Ecom.Application.Favorites.Services
{
    public interface IFavoriteService
    {
        Task<IReadOnlyList<FavoriteProductDto>> GetUserFavoritesAsync(string userId);
        Task<bool> AddFavoriteAsync(string userId, AddFavoriteDto dto);
        Task<bool> RemoveFavoriteAsync(string userId, int productId);
        Task<bool> IsFavoriteAsync(string userId, int productId);
    }
}