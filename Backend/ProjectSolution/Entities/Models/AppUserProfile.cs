using Entities.Enums;

namespace Entities.Models
{
    public class AppUserProfile : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Mail { get; set; }
        public UserRoles Role { get; set; } 

        public virtual AppUser User { get; set; }

        public AppUserProfile()
        {
            Role = UserRoles.User;
        }
    }
}
