namespace SD_330_W22SD_Assignment.Models
{
    public class Vote
    {
        public int ID { get; set; }
        public int VoteScore { get; set; }
        public int? QuestionID { get; set; }
        public int? AnswerID { get; set; }
        public Question? Question { get; set; }
        public Answer? Answer { get; set; }
        public WebUser WebUser { get; set; }
    }
}
