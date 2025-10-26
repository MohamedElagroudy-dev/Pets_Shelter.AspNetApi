using Application.Ratings.DTOs;
using Core.Entities.Product;
using Core.Entities;
using System;

namespace Application.Ratings.Mappings
{
    public static class RatingMappingExtensions
    {
        public static ReturnRatingDto ToDto(this Rating rating)
        {
            return new ReturnRatingDto
            {
                Id = rating.Id,
                Stars = rating.Stars,
                Content = rating.Content,
                UserName = rating.AppUser?.UserName ?? "Unknown",
                ReviewTime = rating.Review
            };
        }

        public static Rating ToEntity(this RatingDto dto, string appUserId)
        {
            return new Rating
            {
                ProductId = dto.ProductId,
                Stars = dto.Stars,
                Content = dto.Content,
                AppUserId = appUserId,
                Review = DateTime.UtcNow
            };
        }
    }
}
