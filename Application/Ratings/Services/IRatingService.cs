using Application.Ratings.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ratings.Services
{
    public interface IRatingService
    {
        Task<bool> AddRatingAsync(RatingDto ratingDto);
        Task<IReadOnlyList<ReturnRatingDto>> GetRatingsForProductAsync(int productId);
        Task DeleteAsync(int id);
    }
}
