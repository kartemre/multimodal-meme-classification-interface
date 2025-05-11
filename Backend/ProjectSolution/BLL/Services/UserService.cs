using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Validators;
using DAL.Interfaces;
using Entities.Enums;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            var validator = new RegisterRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new BusinessException($"Doğrulama başarısız oldu: {errors}");
            }

            // Kullanıcı adı kontrolü
            if (await _userRepository.ExistsAsync(request.Username))
                throw new BusinessException("Kullanıcı adı zaten mevcut.");

            // Kullanıcı oluşturma
            var user = new AppUser
            {
                UserName = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Profile = new AppUserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Mail = request.Email,
                    PhoneNumber = request.Phone,
                    Role = Entities.Enums.UserRoles.User
                }
            };

            await _userRepository.AddAsync(user);
            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new BusinessException("Invalid username or password.");

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Token:ExpiryInMinutes"])),
                Role = user.Profile?.Role.ToString() ?? "Unknown"
            };
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new BusinessException("User not found.");

            return new UserProfileDto
            {
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                Username = user.UserName,
                Email = user.Profile.Mail,
                Phone = user.Profile.PhoneNumber,
                Role = UserRoles.User,
                CreatedAt = user.CreatedTime
            };
        }

        public async Task<bool> UpdateProfileAsync(UserProfileDto userProfile)
        {
            var user = await _userRepository.GetByIdAsync(userProfile.Id);
            if (user == null)
                throw new BusinessException("User not found.");

            user.Profile.FirstName = userProfile.FirstName;
            user.Profile.LastName = userProfile.LastName;
            user.Profile.Mail = userProfile.Email;
            user.Profile.PhoneNumber = userProfile.Phone;
            user.UserName = userProfile.Username;
            user.UpdatedTime = DateTime.Now;
            user.Profile.UpdatedTime = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePassword)
        {
            var user = await _userRepository.GetByIdAsync(changePassword.Id);
            if (user == null)
                throw new BusinessException("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, user.Password))
                throw new BusinessException("Current password is incorrect.");

            if (BCrypt.Net.BCrypt.Verify(changePassword.NewPassword, user.Password))
                throw new BusinessException("New password cannot be the same as the current password.");

            if (BCrypt.Net.BCrypt.Verify(changePassword.NewPassword, user.PreviousPassword))
                throw new BusinessException("New password cannot be the same as the previous password.");

            user.PreviousPassword = user.Password;
            user.UpdatedTime = DateTime.Now;
            user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        private string GenerateJwtToken(AppUser user)
        {
            var securityKey = _configuration["Token:SecurityKey"];
            var issuer = _configuration["Token:Issuer"];
            var audience = _configuration["Token:Audience"];
            var expiryMinutes = double.Parse(_configuration["Token:ExpiryInMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("username", user.UserName),
                new Claim("userId", user.Id.ToString())
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
    }
}
