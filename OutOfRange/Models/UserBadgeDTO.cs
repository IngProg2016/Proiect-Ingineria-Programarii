using System;

namespace OutOfRange.Models
{
    public class UserBadgeDTO
    {
        public UserBadgeDTO()
        {
            
        }

        public UserBadgeDTO(UserBadge badge)
        {
            Badge=BadgeDTO.FromEntity(badge.Badge);
            Obtained=badge.Obtained;
        }

        public DateTime? Obtained { get; set; }

        public BadgeDTO Badge { get; set; }

        public static UserBadgeDTO FromEntity(UserBadge badge)
        {
            return new UserBadgeDTO(badge);
        }
    }
}