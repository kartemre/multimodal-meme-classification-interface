using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserListDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id);

        Task<List<AdminPostDto>> GetAllPostsAsync();
        Task<bool> DeletePostAsync(int id);

        Task<LoginResponseDto> AdminLoginAsync(LoginRequestDto request);
        Task<List<AdminPostDto>> GetOffensivePostsAsync();
    }
} 