using API.Helper;
using Application.Admin.DTO;
using Application.Admin.Services;
using Application.Common;
using Application.Common.Pagination;
using Application.Orders.DTOs;
using Application.Orders.Services;
using Application.Payment.Services;
using Core.Constants;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecom.Application.AnimalApplications.DTOs;
using Ecom.Application.AnimalApplications.Services;
using Ecom.Application.Animals.Services;
using Ecom.Application.Animals.DTOs;
using Ecom.Application.FosterAnimals.Services;
using Ecom.Application.FosterAnimals.DTOs;


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
        private readonly IAnimalApplicationService _Applicationservice;
        private readonly IAnimalService _animalService;
        private readonly IFosterAnimalService _fosterAnimalService;

        public AdminController(IOrderService orderService, IPaymentAppService paymentService, IAdminAppService adminAppService, IAnimalApplicationService service, IAnimalService animalService, IFosterAnimalService fosterAnimalService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _adminAppService = adminAppService;
            _Applicationservice = service;
            _animalService = animalService;
            _fosterAnimalService = fosterAnimalService;
        }
        [HttpGet("orders")]
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
        [HttpGet("orders/{id}")]
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


        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAll([FromQuery] AnimalApplicationParams @params)
        {
            try
            {
                @params.ApplicationType = ApplicationType.Adoption;
                var result = await _Applicationservice.GetAllAsync(@params);
                return Ok(new ResponseAPI<AnimalApplicationStatsResult>(200, "Applications fetched", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }
        [HttpGet("GetAllFosterApplications")]
        public async Task<IActionResult> GetAllFoster([FromQuery] AnimalApplicationParams @params)
        {
            try
            {
                @params.ApplicationType = ApplicationType.Foster;
                var result = await _Applicationservice.GetAllAsync(@params);
                return Ok(new ResponseAPI<AnimalApplicationStatsResult>(200, "Applications fetched", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("GetApplication/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var app = await _Applicationservice.GetByIdAsync(id);
                if (app == null)
                    return NotFound(new ResponseAPI<string>(404, $"Application with ID {id} not found"));

                return Ok(new ResponseAPI<AnimalApplicationDetailsDto>(200, data: app));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("adopted/animals")]
        public async Task<IActionResult> GetAllAdopted([FromQuery] Application.Common.Pagination.AnimalParams @params)
        {
            try
            {
                var result = await _animalService.GetAllAdoptedAsync(@params);
                return Ok(new ResponseAPI<PagedResult<AnimalDTO>>(200, "Adopted animals fetched", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("fostered/animals")]
        public async Task<IActionResult> GetAllFostered([FromQuery] Application.Common.Pagination.AnimalParams @params)
        {
            try
            {
                var result = await _fosterAnimalService.GetAllFosteredAsync(@params);
                return Ok(new ResponseAPI<PagedResult<FosterAnimalDTO>>(200, "Fostered animals fetched", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("fostered/ended")]
        public async Task<IActionResult> GetAllFosterEnded([FromQuery] Application.Common.Pagination.AnimalParams @params, [FromQuery] FosterStatus? status)
        {
            try
            {
                var result = await _fosterAnimalService.GetAllFosterEndedAsync(@params,status);
                return Ok(new ResponseAPI<PagedResult<FosterAnimalDTO>>(200, "Foster records with ended date fetched", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("adopted/animals/{id:int}")]
        public async Task<IActionResult> GetAdoptedAnimalDetails(int id)
        {
            try
            {
                var result = await _animalService.GetAdoptedAnimalWithUserAsync(id);
                if (result == null)
                    return NotFound(new ResponseAPI<string>(404, $"Adopted animal with ID {id} not found"));

                return Ok(new ResponseAPI<AnimalWithUserDTO>(200, "Adopted animal details fetched", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("fostered/animals/{id:int}")]
        public async Task<IActionResult> GetFosteredAnimalDetails(int id)
        {
            try
            {
                var result = await _fosterAnimalService.GetFosteredAnimalWithUserAsync(id);
                if (result == null)
                    return NotFound(new ResponseAPI<string>(404, $"Fostered animal with ID {id} not found"));

                return Ok(new ResponseAPI<FosterAnimalWithUserDTO>(200, "Fostered animal details fetched", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpPost("applications/{id:int}/reject")]
        public async Task<IActionResult> RejectApplication(int id, [FromBody] RejectApplicationDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.AdminNotes))
                    return BadRequest(new ResponseAPI<string>(400, "Admin notes are required for rejection"));

                var app = await _Applicationservice.RejectApplicationAsync(id, request);
                return Ok(new ResponseAPI<AnimalApplicationDetailsDto>(200, "Application rejected successfully", app));
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

        [HttpPost("applications/{id:int}/accept")]
        public async Task<IActionResult> AcceptApplication(int id, [FromBody] AcceptApplicationDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.AdminNotes))
                    return BadRequest(new ResponseAPI<string>(400, "Admin notes are required for acceptance"));

                var app = await _Applicationservice.AcceptApplicationAsync(id, request);
                return Ok(new ResponseAPI<AnimalApplicationDetailsDto>(200, "Application accepted successfully", app));
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

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            try
            {
                var details = await _adminAppService.GetUserDetailsAsync(id);
                if (details == null) return NotFound(new ResponseAPI<string>(404, $"User with ID {id} not found"));
                return Ok(new ResponseAPI<UserDetailsDto>(200, "User details fetched", details));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseAPI<string>(500, ex.Message));
            }
        }

    }
}
