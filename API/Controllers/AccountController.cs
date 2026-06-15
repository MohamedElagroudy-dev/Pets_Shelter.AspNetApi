using Application.Account;
using Application.Account.DTOs;
using Application.Account.DTOs.Application.Account;
using Application.Account.Services;
using Core.Constants;
using Core.Sharing.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<UserInfoDto>> UpdateProfile([FromBody] Application.Account.DTOs.ProfileUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Profile data is required");

            try
            {
                var updated = await _accountAppService.UpdateProfile(dto);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] RestPasswordModel model)
        {
            if (model == null)
                return BadRequest(new { message = "Request body is required" });

            try
            {
                // Decode token in case it was URL-encoded in the link
                if (!string.IsNullOrEmpty(model.Token))
                    model.Token = WebUtility.UrlDecode(model.Token);

                var result = await _accountAppService.ResetPassword(model);
                if (result == null)
                    return NotFound(new { message = "User not found" });

                if (result == "done")
                    return Ok(new { message = "Password has been reset" });

                return BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> forget(string email)
        {
            if (email == null)
                return BadRequest(new { message = "Request body is required" });
            try
            {
                var result = await _accountAppService.SendEmailForForgetPassword(email);
                if (!result)
                    return NotFound(new { message = "User not found" });
                return Ok(new { message = "Password reset link has been sent to your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost("forget-password")]
        [Authorize]
        public async Task<IActionResult> ForgetPassword([FromBody] ChangePasswordDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required" });

            try
            {
                var result = await _accountAppService.ChangePasswordAsync(dto);
                
                if (result == null)
                    return Ok(new { message = "Password has been changed successfully" });

                return BadRequest(new { message = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("activate")]
        public async Task<IActionResult> Activate([FromBody] ActiveAccountModel model)
        {
            if (model == null)
                return BadRequest(new { message = "Request body is required" });

            try
            {
                // Decode token in case it was URL-encoded in the link
                if (!string.IsNullOrEmpty(model.Token))
                    model.Token = WebUtility.UrlDecode(model.Token);

                var success = await _accountAppService.ActiveAccount(model);
                if (success)
                    return Ok(new { message = "Account activated" });

                return BadRequest(new { message = "Activation failed or token invalid" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
