namespace SD_330_W22SD_Assignment.Models
{
    public class Answer
    {
        public int ID { get; set; }
        public string Detail { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
