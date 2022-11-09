using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace simplemeet.Models
{
    public class User
    {
        public User()
        {
            this.Topics = new HashSet<Topic>();
            this.Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ProfileImage { get; set; }
        [NotMapped]
        [DisplayName("Upload Profile Image")]
        public IFormFile? ImageFile { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
