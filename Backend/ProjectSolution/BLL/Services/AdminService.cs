using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Entities.Enums;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;

        public AdminService(
            IAdminRepository adminRepository,
            IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> AdminLoginAsync(LoginRequestDto request)
        {
            var admin = await _adminRepository.GetAdminByUsernameAsync(request.Username);
            
            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
                throw new BusinessException("Invalid username or password.");

            if (admin.Profile?.Role != UserRoles.Admin)
                throw new BusinessException("Access denied. Admin privileges required.");

            var token = GenerateJwtToken(admin);

            return new LoginResponseDto
            {
                Token = token,
                Expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Token:ExpiryInMinutes"])),
                Role = admin.Profile?.Role.ToString() ?? "Unknown"
            };
        }

        private string GenerateJwtToken(AppUser admin)
        {
            var securityKey = _configuration["Token:SecurityKey"];
            var issuer = _configuration["Token:Issuer"];
            var audience = _configuration["Token:Audience"];
            var expiryMinutes = double.Parse(_configuration["Token:ExpiryInMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("username", admin.UserName),
                new Claim("userId", admin.Id.ToString()),
                new Claim("role", "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            var users = await _adminRepository.GetAllUsersAsync();
            return users.Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Profile?.Mail ?? string.Empty,
                IsActive = u.IsActive,
                IsDeleted = u.DeletedTime.HasValue,
                CreatedAt = u.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ss")
            }).ToList();
        }

        public Task<bool> DeleteUserAsync(int id) => _adminRepository.DeleteUserAsync(id);
        public Task<bool> ToggleUserStatusAsync(int id) => _adminRepository.ToggleUserStatusAsync(id);

        public async Task<List<AdminPostDto>> GetAllPostsAsync()
        {
            var posts = await _adminRepository.GetAllPostsAsync();
            return posts.Select(p => new AdminPostDto
            {
                Id = p.Id,
                Title = p.Text.Length > 50 ? p.Text.Substring(0, 47) + "..." : p.Text,
                Author = p.User?.UserName ?? "Unknown",
                Category = "General", // You might want to add a category field to your Post model
                Status = p.IsActive ? "published" : "draft",
                CreatedAt = p.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                ImageBase64 = p.ImageData
            }).ToList();
        }

        public Task<bool> DeletePostAsync(int id) => _adminRepository.DeletePostAsync(id);
    }
} 