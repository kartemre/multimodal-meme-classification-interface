using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAdminRepository
    {
        Task<AppUser> GetAdminByUsernameAsync(string username);
        Task<bool> IsAdminAsync(int userId);
        Task<List<AppUser>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id);

        Task<List<Post>> GetAllPostsAsync();
        Task<bool> DeletePostAsync(int id);
        Task<List<Post>> GetPostsByActiveStatusAsync(bool isActive);
    }
} 