using API.Helper;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.Animal;
using Core.Exceptions;
using Ecom.Application.Animals.DTOs;
using Ecom.Application.Animals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;

        public AnimalsController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnimalParams animalParams)
        {
            try
            {
                var animals = await _animalService.GetAllAsync(animalParams);
                return Ok(new ResponseAPI<PagedResult<AnimalDTO>>(200, "Animals fetched successfully", animals));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAll([FromQuery] AnimalParams animalParams)
        {
            try
            {
                var animals = await _animalService.GetAllMyAsync(animalParams);
                return Ok(new ResponseAPI<PagedResult<AnimalDTO>>(200, "Animals fetched successfully", animals));
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
                var animal = await _animalService.GetAnimalAsync(id);
                return Ok(new ResponseAPI<AnimalDTO>(200, "Animal found", animal));
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

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Add([FromForm] AddAnimalDTO dto)
        {
            try
            {
                var animal = await _animalService.AddAsync(dto);
                return Ok(new ResponseAPI<AnimalDTO>(200, "Animal added successfully", animal));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
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
        public async Task<IActionResult> Update(int id, [FromForm] UpdateAnimalDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new ResponseAPI<string>(400, "Id mismatch"));

                var success = await _animalService.UpdateAsync(dto);

                if (!success)
                    return NotFound(new ResponseAPI<string>(404, $"Animal with Id={id} not found"));

                return Ok(new ResponseAPI<bool>(200, "Animal updated successfully", true));
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

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var animal = await _animalService.DeleteAsync(id);
                return Ok(new ResponseAPI<AnimalDTO>(200, "Animal deleted successfully", animal));
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
    }
}
