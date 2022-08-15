using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;

namespace SD_330_W22SD_Assignment.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var _context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<string> roles = new List<string>
            {
                "Administrator", "User", "User"
            };

            if (!_context.Roles.Any())
            {
                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                _context.SaveChanges();
            }

            if (_context.Question.Any())
            {
                return;
            }

            var questions = new Question[]
            {
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 1" , Title = "Title of question 1", UserID = 1},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 2" , Title = "Title of question 2", UserID = 1},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 3" , Title = "Title of question 3", UserID = 1},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 4" , Title = "Title of question 4", UserID = 2},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 5" , Title = "Title of question 5", UserID = 2},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 6" , Title = "Title of question 6", UserID = 2},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 7" , Title = "Title of question 7", UserID = 3},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 8" , Title = "Title of question 8", UserID = 3},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 9" , Title = "Title of question 9", UserID = 3},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 10" , Title = "Title of question 10", UserID = 4},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 11" , Title = "Title of question 11", UserID = 4},
                new Question{AnswerNum=0,CreatedDate=DateTime.Now,QuestionDetail="This is detail of question 12" , Title = "Title of question 12", UserID = 4}
            };
            foreach(Question q in questions)
            {
                _context.Question.Add(q);
            }
            _context.SaveChanges();

            var answers = new Answer[]
            {
                new Answer{Detail="Detail of answer 1"},
                new Answer{Detail="Detail of answer 2"},
                new Answer{Detail="Detail of answer 3"},
                new Answer{Detail="Detail of answer 4"},
            };
            foreach(Answer answer in answers)
            {
                _context.Answers.Add(answer);
            }
            _context.SaveChanges();

            var comments = new Comment[]
            {
                new Comment{Detail="Detail of comment 1"},
                new Comment{Detail="Detail of comment 2"},
                new Comment{Detail="Detail of comment 3"},
                new Comment{Detail="Detail of comment 4"},
            };
            foreach(Comment comment in comments)
            {
                _context.Comments.Add(comment);
            }
            _context.SaveChanges();

            var tags = new Tag[]
            {
                new Tag{Name="Science"},
                new Tag{Name="Art"},
                new Tag{Name="Music"},
                new Tag{Name="Sport"},
            };
            foreach(Tag tag in tags)
            {
                _context.Tags.Add(tag);
            }
            _context.SaveChanges();

            var votes = new Vote[]
            {
                new Vote{VoteScore=1,QuestionID=1},
                new Vote{VoteScore=-1,QuestionID=2},
                new Vote{VoteScore=1,QuestionID=3},
                new Vote{VoteScore=-1,AnswerID=1},
                new Vote{VoteScore=1,AnswerID=2},
                new Vote{VoteScore=-1,AnswerID=3},
            };
            foreach(Vote vote in votes)
            {
                _context.Votes.Add(vote);
            }
            _context.SaveChanges();

            var webUsers = new WebUser[]
            {
                new WebUser{Name="User7",Reputation=0},
                new WebUser{Name="User2",Reputation=0},
                new WebUser{Name="User3",Reputation=0},
                new WebUser{Name="User4",Reputation=0},
            };
            foreach(WebUser webUser in webUsers)
            {
                _context.WebUsers.Add(webUser);
            }
            _context.SaveChanges();


            if (!_context.Users.Any())
            {
                ApplicationUser seededUser = new ApplicationUser
                {
                    User  = new WebUser { Name = "User4", Reputation = 0 }
                };
            
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(seededUser, "Pass123!");
                seededUser.PasswordHash = hashed;

                await userManager.CreateAsync(seededUser);
                await userManager.AddToRoleAsync(seededUser, "Administrator");
            }
            await _context.SaveChangesAsync();

        }
    }
}
