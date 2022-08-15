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
            int pageSize = 2;
            return _context.Question != null ? 
                          View(await PaginatedList<Question>.CreateAsync(questions.Include(q => q.user), pageNumber ?? 1, pageSize)) :
                          Problem("Entity set 'ApplicationDbContext.Question'  is null.");
        }

        // GET: Questions/Details/5
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

            //if (question.CorrectAnswer is not null)
            //{
            //    DetailsViewModel vm = new DetailsViewModel(question.CorrectAnswer,))
            //}
            return View(question);
        }

        // GET: Questions/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title","QuestionDeatail")] Question question)
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

        [Authorize]
        public IActionResult Answer()
        {
            return View();
        }

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
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Error","Home");
                }
                
            }
            return RedirectToAction("Index");
        }

        public IActionResult SameTagQuestion(string tagName) {
            Tag t = _context.Tags.First(t => t.Name == tagName);
            ViewData["tagName"] = tagName;
            ICollection<Question> questions = _context.Question.Where(q => q.Tags.Contains(t)).Include(q=>q.user).ToList();
            return View(questions);
        }

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
            return View(await _context.Question.Include(q=>q.user).OrderByDescending(q => q.CreatedDate).ToListAsync());
        }

        private bool QuestionExists(int id)
        {
          return (_context.Question?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
