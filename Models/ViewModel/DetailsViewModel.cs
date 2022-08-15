namespace SD_330_W22SD_Assignment.Models.ViewModel
{
    public class DetailsViewModel
    {
        public Answer Answer { get; set; }
        public Question Question { get; set; }
        public DetailsViewModel(Answer answer,Question question)
        {
            Answer = answer;
            Question = question;
        }
    }
}
