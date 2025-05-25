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
            var user = await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;
            user.IsActive = false;
            user.UpdatedTime = System.DateTime.UtcNow;
            user.DeletedTime = System.DateTime.UtcNow;
            if (user.Profile != null)
            {
                user.Profile.IsActive = false;
                user.Profile.UpdatedTime = System.DateTime.UtcNow;
                _context.AppUserProfiles.Update(user.Profile);
            }
            var posts = await _context.Posts.Where(p => p.UserId == id).ToListAsync();
            foreach (var post in posts)
            {
                post.IsActive = false;
                post.UpdatedTime = System.DateTime.UtcNow;
            }
            _context.AppUsers.Update(user);
            _context.Posts.UpdateRange(posts);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _context.AppUsers
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            user.UpdatedTime = System.DateTime.UtcNow;
            if (user.IsActive)
            {
                user.DeletedTime = null;
            }
            else
            {
                user.DeletedTime = System.DateTime.UtcNow;
            }

            if (user.Profile != null)
            {
                user.Profile.IsActive = user.IsActive;
                user.Profile.UpdatedTime = System.DateTime.UtcNow;
                _context.AppUserProfiles.Update(user.Profile);
            }

            var posts = await _context.Posts.Where(p => p.UserId == id).ToListAsync();
            foreach (var post in posts)
            {
                post.IsActive = user.IsActive;
                post.UpdatedTime = System.DateTime.UtcNow;
            }
            _context.Posts.UpdateRange(posts);

            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;
            post.IsActive = false;
            post.DeletedTime = System.DateTime.UtcNow;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Post>> GetPostsByActiveStatusAsync(bool isActive)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsActive == isActive)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync();
        }
    }
} 