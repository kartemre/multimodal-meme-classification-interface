using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string username);
        Task<AppUser> GetByIdAsync(int userId);
        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByResetTokenAsync(string token);
        Task AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
    }
}
