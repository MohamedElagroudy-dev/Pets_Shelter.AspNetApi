using API.Helper;
using Application.Favorites.DTOs;
using Core.Entities;
using Core.Exceptions;
using Ecom.Application.Favorites.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private string GetCurrentUserId()
        {
            var userId = User.FindFirst("uid")?.Value 
                         ?? throw new InvalidOperationException("User Id not found in token");

            return userId;
        }

        // GET: api/favorites
        [HttpGet]
        public async Task<IActionResult> GetUserFavorites()
        {
            try
            {
                var userId = GetCurrentUserId();
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId);

                return Ok(new ResponseAPI<IReadOnlyList<FavoriteProductDto>>(200, "User favorites fetched successfully", favorites));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        // POST: api/favorites
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _favoriteService.AddFavoriteAsync(userId, dto);

                return CreatedAtAction(nameof(GetUserFavorites), new ResponseAPI<bool>(201, "Product added to favorites", true));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        // DELETE: api/favorites/123
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var removed = await _favoriteService.RemoveFavoriteAsync(userId, productId);

                return Ok(new ResponseAPI<bool>(200, removed ? "Favorite Removed" : "Favorite not found", removed));

            }
            catch (NotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        // GET: api/favorites/check/123
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> CheckIfFavorite(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isFavorite = await _favoriteService.IsFavoriteAsync(userId, productId);

                return Ok(new ResponseAPI<bool>(200, isFavorite ? "Product is a favorite" : "Product is not a favorite", isFavorite));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }
    }
}