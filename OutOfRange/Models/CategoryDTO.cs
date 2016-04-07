using System;

namespace OutOfRange.Models
{
    public class CategoryDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CategoryDTO()
        {
            
        }

        public CategoryDTO(Category category)
        {
            ID = category.ID;
            Name = category.Name;
            Description = category.Description;
        }
    }
}