using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Enums;

namespace BLL.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public UserRoles Role { get; set; } // Role özelliği eklendi

        public DateTime CreatedAt { get; set; }
    }

}
