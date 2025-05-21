using System;

namespace BLL.DTOs
{
    public class AdminPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string ImageBase64 { get; set; }
    }
} 