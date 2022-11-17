namespace simplemeet.Models
{
    public class Vote
    {
        public bool Choice { get; set; } // yes or no
        public int Id { get; set; }
        public int User { get; set; }
        public int UserId { get; set; }
        public int Topic { get; set; }
        public int TopicId { get; set; }

    }
}
