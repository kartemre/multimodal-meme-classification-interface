using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string Role { get; set; }
    }
}
