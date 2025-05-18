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
    [Authorize] // Tüm controller'ı authorize yapıyoruz
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpPost("create")]
        [Authorize] // Açıkça belirtiyoruz
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto request)
        {
            try
            {
                var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized(new { error = "User ID not found in token." });

                request.UserId = userId; // Kullanıcı ID'sini DTO'ya ekliyoruz
                
                var result = await _postService.CreatePostAsync(request);
                return Ok(new { 
                    success = true, 
                    message = "Post başarıyla oluşturuldu.",
                    data = result 
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Post oluşturma hatası: {ex.Message}");
                return StatusCode(500, new { error = "Post oluşturulurken bir hata oluştu." });
            }
        }

        [HttpGet("all")]
        [Authorize] // Açıkça belirtiyoruz
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                return Ok(new { 
                    success = true, 
                    data = posts 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Postları getirme hatası: {ex.Message}");
                return StatusCode(500, new { error = "Postlar getirilirken bir hata oluştu." });
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUserPosts()
        {
            try
            {
                var userId = int.Parse(User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized(new { error = "User ID not found in token." });

                var posts = await _postService.GetPostsByUserIdAsync(userId);
                return Ok(new { 
                    success = true, 
                    data = posts 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kullanıcı postlarını getirme hatası: {ex.Message}");
                return StatusCode(500, new { error = "Kullanıcı postları getirilirken bir hata oluştu." });
            }
        }
    }
} 