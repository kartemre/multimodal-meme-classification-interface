using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IPostService
    {
        /// <summary>
        /// Yeni bir post oluşturur
        /// </summary>
        /// <param name="createPostDto">Post oluşturma bilgileri</param>
        /// <returns>Oluşturulan post bilgisi</returns>
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto);

        /// <summary>
        /// Tüm postları getirir
        /// </summary>
        /// <returns>Post listesi</returns>
        Task<List<PostDto>> GetAllPostsAsync();

        /// <summary>
        /// Belirtilen kullanıcının postlarını getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Kullanıcının postları</returns>
        Task<List<PostDto>> GetPostsByUserIdAsync(int userId);
    }
} 