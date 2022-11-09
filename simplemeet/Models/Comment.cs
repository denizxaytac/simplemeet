using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simplemeet.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string? Content { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [Required]
        public Topic? Topic { get; set; }
        public int TopicId { get; set; }

        [NotMapped]
        [Required]
        public User? User { get; set; }
        public int UserId { get; set; }


    }
}
