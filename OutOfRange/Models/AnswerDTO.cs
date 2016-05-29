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

        public AnswerDTO(Answer answer,int dtolevel=0)
        {
            ID = answer.ID;
            UserID = answer.UserID;
            QuestionID = answer.QuestionID;
            AnswerBody = answer.AnswerBody;
            Score = answer.Score;
            Added = answer.Added;
            AspNetUser = new AspNetUserDTO(answer.AspNetUser);
            if(dtolevel==0)
            Comments = answer.Comments.Select(CommentDTO.FromEntity).ToList();
            Accepted = answer.Accepted;
            //Comments = (from com in answer.Comments select new CommentDTO(com)).ToList();
        }

        public Guid ID { get; set; }
        public string UserID { get; set; }
        public Guid QuestionID { get; set; }
        public string AnswerBody { get; set; }
        public int Score { get; set; }
        public bool Accepted { get; set; }
        public DateTime Added { get; set; }
        public AspNetUserDTO AspNetUser { get; set; }
        public QuestionDTO Question { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public int ScoreGiven { get; set; }

        public static AnswerDTO FromEntity(Answer answer,int dtolevel=0)
        {
            return new AnswerDTO(answer,dtolevel);
        }
    }
}