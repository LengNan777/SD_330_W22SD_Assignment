namespace SD_330_W22SD_Assignment.Models
{
    public class WebUser
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Reputation { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
