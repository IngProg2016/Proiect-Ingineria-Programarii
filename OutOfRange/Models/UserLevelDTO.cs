using System;
using System.Collections.Generic;
using System.Linq;

namespace OutOfRange.Models
{
    public class UserLevelDTO
    {
        public UserLevelDTO()
        {
            
        }

        public UserLevelDTO(UserLevel level)
        {
            Level=level.Level;
            XP=level.XP;
            Category=CategoryDTO.FromEntity(level.Category);
            Obtained=level.Obtained;
            UserID = level.UserID;
            Badges = level.UserBadges.Select(UserBadgeDTO.FromEntity).ToList();
        }

        public string UserID { get; set; }

        public List<UserBadgeDTO> Badges { get; set; }

        public DateTime Obtained { get; set; }

        public CategoryDTO Category { get; set; }

        public int XP { get; set; }

        public int Level { get; set; }
        public int QuestionNumber { get; set; }
        public int QuestionViews { get; set; }
        public int QuestionScore { get; set; }
        public int AnswerScore { get; set; }
        public int CommentScore { get; set; }
        public int TotalScore { get; set; }

        public static UserLevelDTO FromEntity(UserLevel level)
        {
            return new UserLevelDTO(level);
        }
    }
}