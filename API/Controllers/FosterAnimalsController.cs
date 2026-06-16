using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.Animal;
using Ecom.Application.FosterAnimals.DTOs;
using Ecom.Application.FosterAnimals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FosterAnimalsController : ControllerBase
    {
        private readonly IFosterAnimalService _fosterAnimalService;

        public FosterAnimalsController(IFosterAnimalService fosterAnimalService)
        {
            _fosterAnimalService = fosterAnimalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnimalParams animalParams)
        {
            var animals = await _fosterAnimalService.GetAllAsync(animalParams);
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var animal = await _fosterAnimalService.GetFosterAnimalAsync(id);
            if (animal == null) return NotFound();
            return Ok(animal);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Add([FromForm] AddFosterAnimalDTO dto)
        {
            var animal = await _fosterAnimalService.AddAsync(dto);
            return Ok(animal);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateFosterAnimalDTO dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var success = await _fosterAnimalService.UpdateAsync(dto);
            if (!success) return NotFound();
            return Ok(true);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _fosterAnimalService.DeleteAsync(id);
            if (animal == null) return NotFound();
            return Ok(animal);
        }
    }
}
