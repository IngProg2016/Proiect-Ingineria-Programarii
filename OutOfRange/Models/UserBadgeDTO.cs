namespace OutOfRange.Models
{
    public class UserBadgeDTO
    {
        public UserBadgeDTO()
        {
            
        }

        public UserBadgeDTO(UserBadge badge)
        {
        }

        public static UserBadgeDTO FromEntity(UserBadge badge)
        {
            return new UserBadgeDTO(badge);
        }
    }
}