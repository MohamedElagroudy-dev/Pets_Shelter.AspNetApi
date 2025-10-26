using Application.Cart.Services;
using Core.Entities.Cart;
using API.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartAppService _cartAppService;

        public CartController(ICartAppService cartAppService)
        {
            _cartAppService = cartAppService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(string id)
        {
            try
            {
                var cart = await _cartAppService.GetCartByIdAsync(id);
                return Ok(new ResponseAPI<ShoppingCart>(200, "Cart fetched successfully", cart));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart([FromBody] ShoppingCart cart)
        {
            try
            {
                var updatedCart = await _cartAppService.UpdateCartAsync(cart);
                return Ok(new ResponseAPI<ShoppingCart>(200, "Cart updated successfully", updatedCart));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            try
            {
                var deleted = await _cartAppService.DeleteCartAsync(id);
                if (!deleted)
                    return NotFound(new ResponseAPI<string>(404, $"Cart with id '{id}' not found"));

                return Ok(new ResponseAPI<string>(200, "Cart deleted successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }
    }
}
