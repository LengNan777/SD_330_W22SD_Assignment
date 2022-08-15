using Microsoft.AspNetCore.Identity;

namespace SD_330_W22SD_Assignment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int WebUserId { get; set; }
        public WebUser User { get; set; } = new WebUser{Name = "User4", Reputation = 0};
        public ApplicationUser() : base()
        {

        }
    }
}
