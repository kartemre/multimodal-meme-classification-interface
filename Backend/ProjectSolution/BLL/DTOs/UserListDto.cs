using System;

namespace BLL.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedAt { get; set; }
    }
} 