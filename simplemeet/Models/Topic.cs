using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simplemeet.Models
{
    public class Topic
    {
        public Topic()
        {
            this.Users = new HashSet<User>();
            this.Votes = new HashSet<Vote>();
        }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        [DisplayName("Start date")]
        public DateTime StartTime { get; set; } = DateTime.Now;
        [DisplayName("End date")]
        public DateTime EndTime { get; set; }
        [Required]
        public User? Creator { get; set; }
        public int CreatorId { get; set; }

        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Vote>? Votes { get; set; }
    }
}
