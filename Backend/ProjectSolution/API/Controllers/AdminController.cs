using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _adminService.AdminLoginAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Admin access successful" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (result)
                return Ok(new { message = "Kullanıcı başarıyla silindi" });
            return NotFound(new { message = "Kullanıcı bulunamadı" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var result = await _adminService.ToggleUserStatusAsync(id);
            if (result)
                return Ok(new { message = "Kullanıcı durumu güncellendi" });
            return NotFound(new { message = "Kullanıcı bulunamadı" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _adminService.GetAllPostsAsync();
            return Ok(posts);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _adminService.DeletePostAsync(id);
            if (result)
                return Ok(new { message = "Post başarıyla silindi" });
            return NotFound(new { message = "Post bulunamadı" });
        }
    }
} 