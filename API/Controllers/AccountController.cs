using Application.Account;
using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Application.Account.Services;
using Core.Sharing;
using Core.Sharing.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountAppService;

        public AccountController(IAccountService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthModel>> Register([FromBody] RegisterModel model)
        {
            var result = await _accountAppService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthModel>> Login([FromBody] TokenRequestModel model)
        {
            var result = await _accountAppService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return Unauthorized(result);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("add-role")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
        {
            var result = await _accountAppService.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPost("Unassign-role")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UnassignRole([FromBody] UnassignRoleModel model)
        {
            var result = await _accountAppService.UnassignRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPost("update-Address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> CreateOrUpdate([FromBody] AddressDto dto)
        {
            if (dto == null)
                return BadRequest("Address data is required");

            var result = await _accountAppService.CreateOrUpdateAddress(dto);

            return Ok(result);
        }

        [HttpGet("auth-status")]
        public ActionResult GetAuthState()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }
        [Authorize]
        [HttpGet("userinfo")]
        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
            var userInfo = await _accountAppService.GetUserInfo();

            if (userInfo == null)
                return NotFound(new { Message = "User not found" });

            return Ok(userInfo);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] GetTokenDto model)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Token is required!");

            var result = await _accountAppService.RefreshToken(refreshToken);

            if (result == null || !result.IsAuthenticated)
                return BadRequest(result);

                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _accountAppService.RevokeToken(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }

        [Authorize]
        [HttpPost("picture")]
        public async Task<IActionResult> UpdatePicture([FromForm] UpdatePictureDto dto)
        {
            try
            {
                var pictureUrl = await _accountAppService.UpdatePictureUrlAsync(dto);
                return Ok(new { message = "Picture updated successfully", pictureUrl });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("picture")]
        public async Task<IActionResult> DeletePicture()
        {
            try
            {
                await _accountAppService.DeletePictureUrlAsync();
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("picture")]
        public async Task<IActionResult> GetPicture()
        {
            try
            {
                var pictureUrl = await _accountAppService.GetPictureUrlAsync();
                return Ok(new { pictureUrl });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


    }
}
