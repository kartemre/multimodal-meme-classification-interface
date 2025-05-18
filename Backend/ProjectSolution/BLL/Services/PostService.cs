using BLL.DTOs;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Entities.Models;
using Entities.Enums;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository postRepository,
            IUserRepository userRepository,
            ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(createPostDto.UserId);
                if (user == null)
                    throw new BusinessException("Kullanıcı bulunamadı.");

                var post = new Post
                {
                    Text = createPostDto.Text,
                    ImageData = createPostDto.ImageBase64,
                    UserId = createPostDto.UserId
                };

                await _postRepository.AddAsync(post);

                return new PostDto
                {
                    Id = post.Id,
                    Text = post.Text,
                    ImageBase64 = post.ImageData,
                    UserId = post.UserId,
                    UserName = user.UserName,
                    CreatedAt = post.CreatedTime
                };
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Post oluşturma hatası: {ex.Message}");
                throw new BusinessException("Post oluşturulurken bir hata oluştu.");
            }
        }

        public async Task<List<PostDto>> GetAllPostsAsync()
        {
            try
            {
                var posts = await _postRepository.GetAllAsync();
                return posts.Select(p => new PostDto
                {
                    Id = p.Id,
                    Text = p.Text,
                    ImageBase64 = p.ImageData,
                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    CreatedAt = p.CreatedTime
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Postları getirme hatası: {ex.Message}");
                throw new BusinessException("Postlar getirilirken bir hata oluştu.");
            }
        }

        public async Task<List<PostDto>> GetPostsByUserIdAsync(int userId)
        {
            var posts = await _postRepository.GetByUserIdAsync(userId);
            return posts.Select(p => new PostDto
            {
                Id = p.Id,
                Text = p.Text,
                ImageBase64 = p.ImageData,
                UserId = p.UserId,
                UserName = p.User?.UserName,
                CreatedAt = p.CreatedTime
            }).ToList();
        }
    }
} 