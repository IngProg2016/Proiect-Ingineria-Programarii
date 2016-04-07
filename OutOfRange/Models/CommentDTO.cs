using System;

namespace OutOfRange.Models
{
    public class CommentDTO
    {
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public string UserID { get; set; }
        public string Description { get; set; }
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
            Description = comment.Description;
            Added = comment.Added;
            Modified = comment.Modified;
            AspNetUser=new AspNetUserDTO(comment.AspNetUser);
        }

        public static CommentDTO FromEntity(Comment comment)
        {
            return new CommentDTO(comment);
        }
    }
}