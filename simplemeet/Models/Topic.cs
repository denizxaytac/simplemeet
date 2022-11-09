using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simplemeet.Models
{
    public class Topic
    {
        public Topic()
        {
            this.Users = new HashSet<User>();

        }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        [Required]
        [NotMapped]
        public User? Creator { get; set; }
        public int CreatorId { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

    }
}
