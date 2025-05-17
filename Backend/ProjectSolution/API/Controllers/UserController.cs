using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
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

        /// <summary>
        /// Şifre sıfırlama isteği başlatır ve kullanıcıya email gönderir
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Şifre sıfırlama isteği alındı: {request.Email}");
                await _userService.ForgotPasswordAsync(request);
                
                return Ok(new { 
                    success = true,
                    message = "Eğer bu email adresi sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilecektir." 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Şifre sıfırlama isteği başarısız: {ex.Message}");
                return StatusCode(500, new { 
                    success = false,
                    error = "İşlem sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin." 
                });
            }
        }

        /// <summary>
        /// Şifre sıfırlama token'ının geçerliliğini kontrol eder
        /// </summary>
        [HttpGet("validate-reset-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { 
                        success = false,
                        error = "Token parametresi gereklidir." 
                    });
                }

                var isValid = await _userService.ValidateResetTokenAsync(token);
                return Ok(new { 
                    success = true,
                    isValid = isValid 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token doğrulama hatası: {ex.Message}");
                return BadRequest(new { 
                    success = false,
                    error = "Token doğrulanırken bir hata oluştu." 
                });
            }
        }

        /// <summary>
        /// Yeni şifreyi kaydeder ve şifre sıfırlama işlemini tamamlar
        /// </summary>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            try
            {
                _logger.LogInformation("Şifre sıfırlama isteği işleniyor...");
                var result = await _userService.ResetPasswordAsync(request);

                if (result)
                {
                    _logger.LogInformation("Şifre başarıyla sıfırlandı.");
                    return Ok(new { 
                        success = true,
                        message = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz." 
                    });
                }

                return BadRequest(new { 
                    success = false,
                    error = "Şifre sıfırlama işlemi başarısız oldu." 
                });
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning($"Şifre sıfırlama iş kuralı hatası: {ex.Message}");
                return BadRequest(new { 
                    success = false,
                    error = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Şifre sıfırlama hatası: {ex.Message}");
                return StatusCode(500, new { 
                    success = false,
                    error = "Şifre sıfırlanırken bir hata oluştu. Lütfen daha sonra tekrar deneyin." 
                });
            }
        }
    }
}
