using System;
using System.Collections.Generic;
using System.Linq;

namespace OutOfRange.Models
{
    public class AnswerDTO
    {
        public AnswerDTO()
        {
            Comments = new HashSet<CommentDTO>();
        }

        public AnswerDTO(Answer answer)
        {
            ID = answer.ID;
            UserID = answer.UserID;
            QuestionID = answer.QuestionID;
            Description = answer.Description;
            Score = answer.Score;
            Added = answer.Added;
            AspNetUser = new AspNetUserDTO(answer.AspNetUser);
            Comments = answer.Comments.Select(CommentDTO.FromEntity).ToList();
            //Comments = (from com in answer.Comments select new CommentDTO(com)).ToList();
        }

        public Guid ID { get; set; }
        public string UserID { get; set; }
        public Guid QuestionID { get; set; }
        public string Description { get; set; }
        public int? Score { get; set; }
        public DateTime Added { get; set; }
        public AspNetUserDTO AspNetUser { get; set; }
        public QuestionDTO Question { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }

        public static AnswerDTO FromEntity(Answer answer)
        {
            return new AnswerDTO(answer);
        }
    }
}