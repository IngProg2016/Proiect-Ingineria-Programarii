namespace OutOfRange.Models
{
    public class BadgeDTO
    {
        public BadgeDTO()
        {
            
        }

        public BadgeDTO(Badge badge)
        {
            Name=badge.Name;
            Description=badge.Description;
            ImageLoc = badge.ImageLoc;
            Image=badge.Image;
            Rarity=badge.Rarity;
            Reward=badge.Reward;
            BadgeCode = badge.IdentifierName;
        }

        public string BadgeCode { get; set; }

        public int Reward { get; set; }

        public int Rarity { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageLoc { get; set; }

        public byte[] Image { get; set; }

        public static BadgeDTO FromEntity(Badge badge)
        {
            return new BadgeDTO(badge);
        }
    }
}