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
        public List<UserLevelDTO> UserLevels { get; set; }

        public AspNetUserDTO()
        {
            
        }

        public AspNetUserDTO(AspNetUser aspNetUser,int dtolevel=0)
        {
            if (aspNetUser == null)
                return;
            UserLevels=new List<UserLevelDTO>();
            Id = aspNetUser.Id;
            Email = aspNetUser.Email;
            UserName = aspNetUser.UserName;
            Credits = aspNetUser.Credits;
            if (dtolevel == 0)
            {
                QuestionNumber = aspNetUser.Questions.Count;
                QuestionViews = aspNetUser.Questions.Sum(question => question.QuestionViews.Count);
                QuestionScore = aspNetUser.Questions.Sum(question => question.Score);
                AnswerScore = aspNetUser.Answers.Sum(answer => answer.Score);
                CommentScore = aspNetUser.Comments.Sum(comment => comment.Score);
                TotalScore = QuestionScore + AnswerScore + CommentScore;
                foreach (var level in aspNetUser.UserLevels)
                {
                    UserLevelDTO ulDto = new UserLevelDTO(level);
                    ulDto.QuestionNumber =
                        aspNetUser.Questions.Count(question => question.CategoryID == level.CategoryID);
                    ulDto.QuestionViews =
                        aspNetUser.Questions.Where(question => question.CategoryID == level.CategoryID)
                            .Sum(question => question.QuestionViews.Count);
                    ulDto.QuestionScore =
                        aspNetUser.Questions.Where(question => question.CategoryID == level.CategoryID)
                            .Sum(question => question.Score);
                    ulDto.AnswerScore = aspNetUser.Answers.Where(
                        answer => answer.Question.CategoryID == level.CategoryID)
                        .Sum(answer => answer.Score);
                    int commScore = 0;
                    foreach (var comment in aspNetUser.Comments.ToList())
                    {
                        if (comment.Question != null)
                        {
                            if (comment.Question.CategoryID == level.CategoryID)
                                commScore += comment.Score;
                        }
                        else if (comment.Answer != null)
                        {
                            if (comment.Answer.Question.CategoryID == level.CategoryID)
                                commScore += comment.Score;
                        }
                    }
                    ulDto.CommentScore = commScore;
                    ulDto.TotalScore = ulDto.QuestionScore + ulDto.AnswerScore + ulDto.CommentScore;
                    UserLevels.Add(ulDto);
                }
                UserLevels.Sort((dto, levelDto) => levelDto.XP - dto.XP);
            }
        }

        public static AspNetUserDTO FromEntity(AspNetUser user, int dtoLevel=0)
        {
            return new AspNetUserDTO(user,dtoLevel);
        }
    }
}