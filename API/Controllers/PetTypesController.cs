using API.Helper;
using Application.Categories.DTOs;
using Application.Categories.Services;
using Application.PetTypes.DTOs;
using Application.PetTypes.Services;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetTypesController : ControllerBase
    {
        private readonly IPetTypeService _service;

        public PetTypesController(IPetTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var petTypes = await _service.GetAllAsync();
                return Ok(new ResponseAPI<IEnumerable<PetTypeDTO>>(200, "Pet types fetched successfully", petTypes));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var petType = await _service.GetPetTypeAsync(id);
                return Ok(new ResponseAPI<PetTypeDTO>(200, "Pet type found", petType));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Add(AddPetTypeDTO dto)
        {
            try
            {
                var result = await _service.AddAsync(dto);
                return Ok(new ResponseAPI<PetTypeDTO>(200, "Pet type created successfully", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Update(int id, UpdatePetTypeDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new ResponseAPI<string>(400, "Mismatched pet type ID"));

                var updated = await _service.UpdateAsync(dto);
                return Ok(new ResponseAPI<PetTypeDTO>(200, "Pet type updated successfully", updated));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new ResponseAPI<string>(200, "PetType deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseAPI<string>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }

        [HttpGet("exists/{id}")]
        public async Task<IActionResult> PetTypeExists(int id)
        {
            try
            {
                var exists = await _service.PetTypeExistsAsync(id);
                return Ok(new ResponseAPI<bool>(200, "Check completed", exists));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }


    }
}
