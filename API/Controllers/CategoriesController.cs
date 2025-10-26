using API.Helper;
using Application.Categories.DTOs;
using Application.Categories.Services;
using Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _service.GetAllAsync();
                return Ok(new ResponseAPI<IEnumerable<CategoryDTO>>(200, "Categories fetched successfully", categories));
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
                var category = await _service.GetProductAsync(id);
                return Ok(new ResponseAPI<CategoryDTO>(200, "Category found", category));
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
        public async Task<IActionResult> Add(AddCategoryDTO dto)
        {
            try
            {
                var result = await _service.AddAsync(dto);
                return Ok(new ResponseAPI<CategoryDTO>(200, "Category created successfully", result));
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
        public async Task<IActionResult> Update(int id, UpdateCategoryDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new ResponseAPI<string>(400, "Mismatched category ID"));

                var updated = await _service.UpdateAsync(dto);
                return Ok(new ResponseAPI<CategoryDTO>(200, "Category updated successfully", updated));
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
                return Ok(new ResponseAPI<string>(200, "Category deleted successfully"));
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
        public async Task<IActionResult> CategoryExists(int id)
        {
            try
            {
                var exists = await _service.CategoryExistsAsync(id);
                return Ok(new ResponseAPI<bool>(200, "Check completed", exists));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(500, ex.Message));
            }
        }


    }
}
