namespace SD_330_W22SD_Assignment.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public WebUser User { get; set; }
        public string Detail { get; set; }
    }
}
