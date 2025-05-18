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
    }
} 