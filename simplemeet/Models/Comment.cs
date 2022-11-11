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
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
        [NotMapped]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
