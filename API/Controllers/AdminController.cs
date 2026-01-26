using API.Helper;
using Application.Admin.DTO;
using Application.Admin.Services;
using Application.Common;
using Application.Common.Pagination;
using Application.Orders.DTOs;
using Application.Orders.Services;
using Application.Payment.Services;
using Core.Constants;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Exceptions;
using Core.Interfaces;
using Core.Sharing.Pagination;
using Ecom.Application.Products.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentAppService _paymentService;
        private readonly IAdminAppService _adminAppService;
        public AdminController(IOrderService orderService, IPaymentAppService paymentService, IAdminAppService adminAppService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _adminAppService = adminAppService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OrderParams orderParams)
        {
            try
            {
                var orders = await _orderService.GetAllAsync(orderParams);
                return Ok(new ResponseAPI<PagedResult<OrderDto>>(200, "Orders fetched successfully", orders));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseAPI<OrderDto>>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(new ResponseAPI<OrderDto>(200, data: order));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (NotFoundException)
            {
                return NotFound(new ResponseAPI(404, $"Order with ID {id} not found"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<ResponseAPI<OrderDto>>> RefundOrder(int id)
        {
            try
            {
                var order = await _paymentService.RefundOrderAsync(id);
                return Ok(new ResponseAPI<OrderDto>(200, "Order refunded successfully.", order));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpPut("orders/{id:int}/status")]
        public async Task<ActionResult<ResponseAPI<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
                return Ok(new ResponseAPI<OrderDto>(200, $"Order status updated to {request.Status}", order));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var roles = await _adminAppService.GetAvailableRolesAsync();
                return Ok(new ResponseAPI<IEnumerable<string>>(200, "Roles fetched successfully", roles));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserParams userParams)
        {
            try
            {
                var users = await _adminAppService.GetAllUsersAsync(userParams);
                return Ok(new ResponseAPI<PagedResult<UserDto>>(200, "Users fetched successfully", users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

    }
}
