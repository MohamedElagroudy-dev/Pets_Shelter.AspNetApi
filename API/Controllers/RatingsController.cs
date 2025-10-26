using API.Helper;
using Application.Ratings.DTOs;
using Application.Ratings.Services;
using Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<ResponseAPI>> AddRating(RatingDto dto)
        {
            try
            {
                var result = await _ratingService.AddRatingAsync(dto);
                if (result)
                    return Ok(new ResponseAPI(200, "Rating added successfully"));
                return BadRequest(new ResponseAPI(400, "You already rated this product"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseAPI(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI(500, ex.Message));
            }
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<ResponseAPI<IEnumerable<ReturnRatingDto>>>> GetRatings(int productId)
        {
            try
            {
                var result = await _ratingService.GetRatingsForProductAsync(productId);
                return Ok(new ResponseAPI<IEnumerable<ReturnRatingDto>>(200, "Ratings fetched successfully", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<IEnumerable<ReturnRatingDto>>(500, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Admin}")]
        public async Task<ActionResult<ResponseAPI>> DeleteRating(int id)
        {
            try
            {
                await _ratingService.DeleteAsync(id);
                return Ok(new ResponseAPI(200, "Rating deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ResponseAPI(401, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI(500, ex.Message));
            }
        }
    }
}
