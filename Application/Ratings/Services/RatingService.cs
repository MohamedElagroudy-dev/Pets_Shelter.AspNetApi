using Application.Account;
using Application.Categories.DTOs;
using Application.Ratings.DTOs;
using Application.Ratings.Mappings;
using Core.Entities;
using Core.Entities.Product;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ratings.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<RatingService> _logger;

        public RatingService(IUnitOfWork unitOfWork, IUserContext userContext, ILogger<RatingService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<bool> AddRatingAsync(RatingDto ratingDto)
        {
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            _logger.LogInformation("Adding or updating rating for product {ProductId} by user {Email}", ratingDto.ProductId, currentUser.Email);

            var existing = await _unitOfWork.Ratings
                .GetByAsync(r => r.AppUserId == currentUser.Id && r.ProductId == ratingDto.ProductId);

            if (existing != null)
            {
                existing.Stars = ratingDto.Stars;
                existing.Content = ratingDto.Content;
                await _unitOfWork.Ratings.UpdateAsync(existing.Id, existing);
                _logger.LogInformation("Updated existing rating (Id={RatingId}) for product {ProductId}", existing.Id, ratingDto.ProductId);
            }
            else
            {
                var rating = ratingDto.ToEntity(currentUser.Id);
                await _unitOfWork.Ratings.AddAsync(rating);
                _logger.LogInformation("Created new rating for product {ProductId}", ratingDto.ProductId);
            }

            var product = await _unitOfWork.Products.GetAsync(ratingDto.ProductId);
            if (product != null)
            {
                var ratings = await _unitOfWork.Ratings.GetAllAsync(r => r.ProductId == ratingDto.ProductId);
                if (ratings.Any())
                {
                    double avg = ratings.Average(r => r.Stars);
                    product.rating = Math.Round(avg * 2, MidpointRounding.AwayFromZero) / 2;
                    await _unitOfWork.Products.UpdateAsync(product.Id, product);
                    _logger.LogInformation("Updated product {ProductId} average rating to {Average}", product.Id, product.rating);
                }
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IReadOnlyList<ReturnRatingDto>> GetRatingsForProductAsync(int productId)
        {
            _logger.LogInformation("Fetching ratings for product {ProductId}", productId);

            var ratings = await _unitOfWork.Ratings
                .GetAllAsync(r => r.ProductId == productId, r => r.AppUser);

            return ratings.Select(r => r.ToDto()).ToList();
        }


        public async Task DeleteAsync(int id)
        {
            var currentUser = _userContext.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            var rating = await _unitOfWork.Ratings.GetAsync(id);
            if (rating == null)
                throw new KeyNotFoundException($"Rating with Id={id} not found.");

            if (rating.AppUserId != currentUser.Id)
                throw new UnauthorizedAccessException("You are not allowed to delete this rating.");

            await _unitOfWork.Ratings.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            //  Recalculate product average after deletion

            var productId = rating.ProductId;
            var product = await _unitOfWork.Products.GetAsync(productId);
            if (product != null)
            {
                var ratings = await _unitOfWork.Ratings.GetAllAsync(r => r.ProductId == productId);
                product.rating = ratings.Any()
                    ? Math.Round(ratings.Average(r => r.Stars) * 2, MidpointRounding.AwayFromZero) / 2
                    : 0;
                await _unitOfWork.Products.UpdateAsync(product.Id, product);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Recalculated average rating for product {ProductId} after deleting rating", productId);
            }
        }



    }
}
