using System;

namespace OutOfRange.Models
{
    public class CommentDTO
    {
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public string UserID { get; set; }
        public string CommentBody { get; set; }
        public int Score { get; set; }
        public int ScoreGiven { get; set; }
        public DateTime Added { get; set; }
        public DateTime? Modified { get; set; }
        public AspNetUserDTO AspNetUser { get; set; }

        public CommentDTO()
        {
            
        }

        public CommentDTO(Comment comment)
        {
            ID = comment.ID;
            ParentID = comment.ParentID;
            UserID = comment.UserID;
            Score = comment.Score;
            CommentBody = comment.CommentBody;
            Added = comment.Added;
            Modified = comment.Modified;
            AspNetUser=new AspNetUserDTO(comment.AspNetUser,1);
        }

        public static CommentDTO FromEntity(Comment comment)
        {
            return new CommentDTO(comment);
        }
    }
}