using API.Helper;
using Application.Common;
using Core.Exceptions;
using Core.Sharing;
using Core.Sharing.Pagination;
using Ecom.Application.Products.DTOs;
using Ecom.Application.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await _productService.GetAllAsync(productParams);
                return Ok(new ResponseAPI<PagedResult<ProductDTO>>(200, "Products fetched successfully", products));
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
                var product = await _productService.GetProductAsync(id);
                return Ok(new ResponseAPI<ProductDTO>(200, "Product found", product));
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
        public async Task<IActionResult> Add([FromForm] AddProductDTO dto)
        {
            try
            {
                var product = await _productService.AddAsync(dto);
                return Ok(new ResponseAPI<ProductDTO>(200, "Product added successfully", product));
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
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new ResponseAPI<string>(400, "Id mismatch"));

                var success = await _productService.UpdateAsync(dto);

                if (!success)
                    return NotFound(new ResponseAPI<string>(404, $"Product with Id={id} not found"));

                return Ok(new ResponseAPI<bool>(200, "Product updated successfully", true));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
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

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _productService.DeleteAsync(id);
                return Ok(new ResponseAPI<ProductDTO>(200, "Product deleted successfully", deleted));
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
