using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Models;

namespace SD_330_W22SD_Assignment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SD_330_W22SD_Assignment.Models.Question>? Question { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Comment>? Comments { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Tag>? Tags { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Vote>? Votes { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.WebUser>? WebUsers { get; set; }
        public DbSet<SD_330_W22SD_Assignment.Models.Answer>? Answers { get; set; }
    }
}