using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;
using SD_330_W22SD_Assignment.Models;
using SD_330_W22SD_Assignment.Models.ViewModel;

namespace SD_330_W22SD_Assignment.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public QuestionsController(ApplicationDbContext context, UserManager<ApplicationUser> usrMngr)
        {
            _context = context;
            _userManager = usrMngr;
        }

        // GET: Questions
        //The default action of QuestionsController. Questions would be listed by date order of number of answer order. User could switch mode by click the button.
        //Every page has 10 questions mostly. Click next or previous button to see other question.
        public async Task<IActionResult> Index(string sortOrder,int? pageNumber)
        {   
            int r = 1;
            try
            {
                foreach (Vote v in _context.Votes)
                {
                    if (v.WebUser == _context.WebUsers.First())
                    {
                        r += v.VoteScore;
                    }
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }          
            @ViewData["Reputation"] = r*5;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSort"] = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["AnswerNumSort"] = sortOrder == "AnswerNum" ? "AnswerNumDesc" : "AnswerNum";
            var questions = from q in _context.Question
                            select q;
            //Change the sort principle according to the parameter delivered when user clicked the button.
            switch (sortOrder)
            {
                case "Date":
                    questions = questions.OrderByDescending(q => q.CreatedDate);
                    break;
                case "AnswerNumDesc":
                    questions = questions.OrderBy(q => q.AnswerNum);
                    break;
                case "AnswerNum":
                    questions = questions.OrderByDescending(q => q.AnswerNum);
                    break;
                default:
                    questions = questions.OrderBy(q => q.CreatedDate);
                    break;
            }
            int pageSize = 10;
            return _context.Question != null ? 
                          View(await PaginatedList<Question>.CreateAsync(questions.Include(q => q.user), pageNumber ?? 1, pageSize)) :
                          Problem("Entity set 'ApplicationDbContext.Question'  is null.");
        }

        // GET: Questions/Details/5
        //User click the detail button would pass the id of question to this method, then controller find the question and pass it to view if there is a valid Id.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question.Include(q => q.user).Include(q=>q.Answers).Include(q=>q.Tags)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        //Authorize tag ensures method could not be called with anonymous user.
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        //Use bind method to state which properties user could add to avoid overpost. If the input valid, a new question would be create and pass into database.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title","QuestionDetail")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.AnswerNum = 0;
                question.CreatedDate = DateTime.Now;
                question.UserID = 1;
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        // GET: Questions/Edit/5
        //Find question and pass it to the view, so user could edit, this action was not used in this assignment.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID")] Question question)
        {
            if (id != question.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question.Include(q=>q.user)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        //Like a post method of delete, the name was changed in case of same parameter.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Question == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Question'  is null.");
                }
                var question = await _context.Question.FindAsync(id);
                if (question != null)
                {
                    _context.Question.Remove(question);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("AdministrativePage");
            }catch(Exception ex)
            {
                return RedirectToAction("AdministrativePage");
            }
        }

        //This action would be called when user click "answer this question" button.
        [Authorize]
        public IActionResult Answer()
        {
            return View();
        }

        //Receive the answer from input of user and use it to create a new answer, then add it to the answer list of the question.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(string answer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Question.First().Answers.Add(new Answer { Detail = answer });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Error","Home");
                }
                
            }
            return RedirectToAction("Index");
        }

        //Find all questions with same tag by clicking the tag of question in detail page. Realize by checking all questions and pass them whose tags contain the specified tag to view.
        public IActionResult SameTagQuestion(string tagName) {
            try
            {
                Tag t = _context.Tags.First(t => t.Name == tagName);
                ViewData["tagName"] = tagName;
                ICollection<Question> questions = _context.Question.Where(q => q.Tags.Contains(t)).Include(q => q.user).ToList();
                return View(questions);
            }
            catch(Exception ex)
            {
                return View("Index");
            }
            
        }

        //This method would be called when user click the vote button. A new vote object would be created, if it is a upvote, set the votescore property as 1, if it is a downvote, then set it as -1.
        //If it is vote for question, the question id would be passed and set as QuestionID propery, if it is vote for question, the answer id would be passed and set as AnswerID property.
        //Anonymous user could not vote.
        [Authorize]
        public IActionResult UpVote(int QuestionId,int AnswerId)
        {
            try
            {
                if (QuestionId > 0)
                {
                    _context.Votes.Add(new Vote { VoteScore = 1, QuestionID = QuestionId, WebUser = _context.WebUsers.First() });
                }
                else if (AnswerId > 0)
                {
                    _context.Votes.Add(new Vote { VoteScore = 1, AnswerID = AnswerId, WebUser = _context.WebUsers.First() });
                }
            }catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //This method would be called when user click the vote button. A new vote object would be created, if it is a upvote, set the votescore property as 1, if it is a downvote, then set it as -1.
        //If it is vote for question, the question id would be passed and set as QuestionID propery, if it is vote for question, the answer id would be passed and set as AnswerID property.
        //Anonymous user could not vote.
        [Authorize]
        public IActionResult DownVote(int QuestionId, int AnswerId)
        {
            try
            {
                if (QuestionId > 0)
                {
                    _context.Votes.Add(new Vote { VoteScore = -1, QuestionID = QuestionId, WebUser = _context.WebUsers.First() });
                }
                else if (AnswerId > 0)
                {
                    _context.Votes.Add(new Vote { VoteScore = -1, AnswerID = AnswerId, WebUser = _context.WebUsers.First() });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //Receive two parameter that helps to find which answer would be set as correct answer of which question. If both of them are valid, set the answer object as property of the question.
        [Authorize]
        public IActionResult SetCorrectAnswer(int AnswerId,int QuestionId)
        {
            try
            {
                Answer a = _context.Answers.First(a=>a.ID == AnswerId);
                Question q = _context.Question.First(q => q.ID == QuestionId);
                if(q != null && a!= null)
                {
                    q.CorrectAnswer = a;
                }
            }catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> AdministrativePage()
        {
            try
            {
                return View(await _context.Question.Include(q => q.user).OrderByDescending(q => q.CreatedDate).ToListAsync());
            }
            catch(Exception ex)
            {
                return View("Index");
            }
        }

        //Model default action.
        private bool QuestionExists(int id)
        {
          return (_context.Question?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
