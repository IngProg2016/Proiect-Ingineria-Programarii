using System.Collections.Generic;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace OutOfRange.Models
{
    public class AspNetUserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int QuestionNumber { get; set; }
        public int QuestionViews { get; set; }
        public int QuestionScore { get; set; }
        public int AnswerScore { get; set; }
        public int CommentScore { get; set; }
        public int Credits { get; set; }
        public int TotalScore { get; set; }
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
            if (aspNetUser.Credits != null)
                Credits = decimal.ToInt32(aspNetUser.Credits.Value);
            else
                Credits = 0;
            QuestionNumber = aspNetUser.Questions.Count;
            QuestionViews = aspNetUser.Questions.Sum(question => question.QuestionViews.Count);
            QuestionScore = decimal.ToInt32(aspNetUser.Questions.Sum(question => question.Score));

            foreach (var level in aspNetUser.UserLevels)
            {

            }
        }

        public static AspNetUserDTO FromEntity(AspNetUser user)
        {
            return new AspNetUserDTO(user);
        }
    }
}