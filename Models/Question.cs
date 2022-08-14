using System.ComponentModel;

namespace SD_330_W22SD_Assignment.Models
{
    public class Question
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int UserID { get; set; }
        public WebUser user { get; set; }
        [DisplayName("Answer Number")]
        public int AnswerNum { get; set; }
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        public string QuestionDetail { get; set; }
        public Answer? CorrectAnswer { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
