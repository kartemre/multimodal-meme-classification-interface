using Entities.Models;

namespace DAL.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task<List<Post>> GetAllAsync();
        Task<Post> GetByIdAsync(int id);
        Task UpdateAsync(Post post);
        Task<List<Post>> GetPostsByUserIdAsync(int userId);
    }
} 