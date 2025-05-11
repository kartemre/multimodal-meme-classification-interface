using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileDto> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(UserProfileDto userProfile);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePassword);
    }
}
