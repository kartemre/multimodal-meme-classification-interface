using System;

namespace BLL.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImageBase64 { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
} 