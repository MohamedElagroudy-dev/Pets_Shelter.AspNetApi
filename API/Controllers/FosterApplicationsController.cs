using API.Helper;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Ecom.Application.AnimalApplications.DTOs;
using Ecom.Application.AnimalApplications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FosterApplicationsController : ControllerBase
    {
        private readonly IAnimalApplicationService _service;

        public FosterApplicationsController(IAnimalApplicationService service)
        {
            _service = service;
        }
        private string GetCurrentUserId()
        {
            var userId = User.FindFirst("uid")?.Value
                         ?? throw new InvalidOperationException("User Id not found in token");

            return userId;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> Create([FromBody] CreateAnimalApplicationDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ResponseAPI(401));

                dto.ApplicationType = ApplicationType.Foster;

                var id = await _service.CreateFosterAsync(dto, userId);
                return Ok(new ResponseAPI<int>(200, "Application submitted", id));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] AnimalApplicationParams @params)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ResponseAPI(401));

                @params.ApplicationType = ApplicationType.Foster;
                var result = await _service.GetMyApplicationsAsync(userId, @params);
                return Ok(new ResponseAPI<PagedResult<AnimalApplicationDto>>(200, "Applications fetched", result));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new ResponseAPI(401));

                var app = await _service.GetMyApplicationByIdAsync(userId, id);
                if (app == null)
                    return NotFound(new ResponseAPI<string>(404, $"Application with ID {id} not found"));

                return Ok(new ResponseAPI<AnimalApplicationDetailsDto>(200, $"Applications with id {id} fetched", app));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ResponseAPI(401));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
