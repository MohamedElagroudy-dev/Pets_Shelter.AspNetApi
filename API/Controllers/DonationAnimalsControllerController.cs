using API.Helper;
using Application.Common;
using Application.Common.Pagination;
using Application.donationAnimal.DTOs;
using Application.donationAnimal.Services;
using Core.Constants;
using Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
        public class DonationAnimalsController : ControllerBase
        {
            private readonly IDonationAnimalService _donationAnimalService;

            public DonationAnimalsController(IDonationAnimalService donationAnimalService)
            {
                _donationAnimalService = donationAnimalService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll([FromQuery] DonationAnimalParams animalParams)
            {
                try
                {
                    var animals = await _donationAnimalService.GetAllAsync(animalParams);
                    return Ok(new ResponseAPI<PagedResult<DonationAnimalDTO>>(200, "Donation animals fetched successfully", animals));
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
                    var animal = await _donationAnimalService.GetAnimalAsync(id);
                    return Ok(new ResponseAPI<DonationAnimalDTO>(200, "Donation animal found", animal));
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
            public async Task<IActionResult> Add([FromForm] AddDonationAnimalDTO dto)
            {
                try
                {
                    var animal = await _donationAnimalService.AddAsync(dto);
                    return Ok(new ResponseAPI<DonationAnimalDTO>(200, "Donation animal added successfully", animal));
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
            public async Task<IActionResult> Update(int id, [FromForm] UpdateDonationAnimalDTO dto)
            {
                try
                {
                    if (id != dto.Id)
                        return BadRequest(new ResponseAPI<string>(400, "Id mismatch"));

                    var success = await _donationAnimalService.UpdateAsync(dto);

                    if (!success)
                        return NotFound(new ResponseAPI<string>(404, $"Donation animal with Id={id} not found"));

                    return Ok(new ResponseAPI<bool>(200, "Donation animal updated successfully", true));
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
                    var animal = await _donationAnimalService.DeleteAsync(id);
                    return Ok(new ResponseAPI<DonationAnimalDTO>(200, "Donation animal deleted successfully", animal));
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
