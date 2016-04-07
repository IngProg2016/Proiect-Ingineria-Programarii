using System.Collections.Generic;
using System.Linq;

namespace OutOfRange.Models
{
    public class AspNetUserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public ICollection<UserBadgeDTO> UserBadges { get; set; }
        public ICollection<UserLevelDTO> UserLevels { get; set; }

        public AspNetUserDTO()
        {
            
        }
        public AspNetUserDTO(AspNetUser aspNetUser)
        {
            Id = aspNetUser.Id;
            Email = aspNetUser.Email;
            UserName = aspNetUser.UserName;
        }
    }
}