using DAL.Context;
using DAL.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MyContext _context;

        public AdminRepository(MyContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetAdminByUsernameAsync(string username)
        {
            return await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> IsAdminAsync(int userId)
        {
            var user = await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Profile?.Role == Entities.Enums.UserRoles.Admin;
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Profile)
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null) return false;
            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null) return false;
            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 