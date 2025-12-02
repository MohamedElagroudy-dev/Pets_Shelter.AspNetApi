using Application.Favorites.DTOs;
using Application.Favorites.Mappings;
using Core.Entities;
using Core.Entities.Product;
using Core.Exceptions;
using Core.Interfaces;
using Ecom.Application.Products.Mappings;
using Ecom.Application.Products.DTOs;
using Microsoft.Extensions.Logging;

namespace Ecom.Application.Favorites.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(IUnitOfWork unitOfWork, ILogger<FavoriteService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IReadOnlyList<FavoriteProductDto>> GetUserFavoritesAsync(string userId)
        {
            _logger.LogInformation("Fetching favorites for user {UserId}", userId);

            var favorites = await _unitOfWork.Repository<Favorite>().GetAllAsync(
                f => f.AppUserId == userId,
                f => f.Product,
                f => f.Product.Photos,
                f => f.Product.Category,
                f => f.Product.PetType
            );

            return favorites
                .Where(f => f.Product != null)
                .Select(f => f.Product!.ToFavoriteDto(f.DateAdded))
                .ToList();
        }

        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            _logger.LogInformation("Checking favorite existence for user {UserId} and product {ProductId}", userId, productId);

            var favorite = await _unitOfWork.Repository<Favorite>().GetByAsync(
                f => f.AppUserId == userId && f.ProductId == productId
            );

            return favorite != null;
        }

        public async Task<bool> AddFavoriteAsync(string userId, AddFavoriteDto dto)
        {
            _logger.LogInformation("Adding product {ProductId} to favorites for user {UserId}", dto.ProductId, userId);

            if (await IsFavoriteAsync(userId, dto.ProductId))
            {
                _logger.LogInformation("Product {ProductId} is already a favorite for user {UserId}", dto.ProductId, userId);
                return true;
            }

            var productExists = await _unitOfWork.Repository<Product>().GetAsync(dto.ProductId) != null;
            if (!productExists)
            {
                _logger.LogWarning("Product {ProductId} not found when adding to favorites for user {UserId}", dto.ProductId, userId);
                throw new NotFoundException("Product", dto.ProductId.ToString());
            }

            var newFavorite = new Favorite(userId, dto.ProductId); // Uses the new string constructor

            await _unitOfWork.Repository<Favorite>().AddAsync(newFavorite);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Product {ProductId} successfully added to favorites for user {UserId}", dto.ProductId, userId);
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, int productId)
        {
            _logger.LogInformation("Removing product {ProductId} from favorites for user {UserId}", productId, userId);

            var favoriteToRemove = await _unitOfWork.Repository<Favorite>().GetByAsync(
                f => f.AppUserId == userId && f.ProductId == productId
            );

            if (favoriteToRemove != null)
            {
                // Delete using the entity's ID, which is provided by BaseEntity
                await _unitOfWork.Repository<Favorite>().DeleteAsync(favoriteToRemove.Id);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Product {ProductId} removed from favorites for user {UserId}", productId, userId);
                return true;
            }

            _logger.LogInformation("Favorite not found for product {ProductId} and user {UserId}", productId, userId);
            return false;
        }
    }
}