using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _userService.RegisterAsync(request);
                if (!result)
                    return BadRequest("Kayıt başarısız oldu");

                return Ok(new { message = "Kayıt başarılı." });
            }
            catch (BusinessException ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var loginResponse = await _userService.LoginAsync(request);

                return Ok(new
                {
                    Token = loginResponse.Token,
                    Expiry = loginResponse.Expiry,
                    Role = loginResponse.Role
                });
            }
            catch (BusinessException ex)
            {
                return Unauthorized(new { error = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");

            try
            {
                var profile = await _userService.GetProfileAsync(userId);
                return Ok(profile);
            }
            catch (BusinessException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto userProfile)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Unauthorized(new { error = "User ID not found in token." });

            if (!int.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
                return Unauthorized(new { error = "Invalid user ID in token." });

            userProfile.Id = userId;

            try
            {
                var result = await _userService.UpdateProfileAsync(userProfile);
                if (!result)
                    return BadRequest("Profile update failed.");

                return Ok("Profile updated successfully.");
            }
            catch (BusinessException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            var userIdClaim = User?.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Unauthorized(new { error = "User ID not found in token." });

            if (!int.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
                return Unauthorized(new { error = "Invalid user ID in token." });

            changePassword.Id = userId;

            try
            {
                var result = await _userService.ChangePasswordAsync(changePassword);
                if (!result)
                    return BadRequest("Password change failed.");

                return Ok("Password changed successfully.");
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
