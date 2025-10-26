using API.Helper;
using Application.Orders.DTOs;
using Application.Orders.Services;
using Core.Exceptions;
using Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<ResponseAPI<OrderDto>>> CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(orderDto);
                return Ok(new ResponseAPI<OrderDto>(200, "Order created successfully", order));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseAPI(400, ex.Message));
            }
            catch (Exception )
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseAPI<IReadOnlyList<OrderDto>>>> GetOrdersForUser()
        {
            try
            {
                var orders = await _orderService.GetOrdersForUserAsync();
                return Ok(new ResponseAPI<IReadOnlyList<OrderDto>>(200, data: orders));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (Exception )
            {
                throw; //exception middleware handle it
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseAPI<OrderDto>>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetUserOrderByIdAsync(id);
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
            catch (Exception )
            {
                throw;
            }
        }
    }
}