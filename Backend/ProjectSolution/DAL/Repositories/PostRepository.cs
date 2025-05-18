using DAL.Context;
using DAL.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly MyContext _context;

        public PostRepository(MyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync();
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId && p.IsActive)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            try
            {
                _context.Entry(post).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post: {ex.Message}");
                throw;
            }
        }
    }
} 