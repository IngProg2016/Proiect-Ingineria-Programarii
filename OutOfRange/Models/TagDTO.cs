using System;
using System.Collections.Generic;

namespace OutOfRange.Models
{
    public class TagDTO
    {
        public TagDTO()
        {
            Questions=new HashSet<QuestionDTO>();
        }

        public TagDTO(Tag tag)
        {
            ID = tag.ID;
            Name = tag.Name;
            Description = tag.Description;
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<QuestionDTO> Questions { get; set; }

        public static TagDTO FromEntity(Tag tag)
        {
            return new TagDTO(tag);
        }
    }
}