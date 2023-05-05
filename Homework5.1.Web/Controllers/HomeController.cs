using Homework5._1.Data;
using Homework5._1.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Homework5._1.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;
        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Login()
        {
            return Redirect("/account/login");
        }
        public IActionResult Index()
        {
            QARepository qaR = new(_connectionString);
            var hpvm = new HomePageViewModel()
            {
                Questions = qaR.GetAllQuestions()
            };
            return View(hpvm);
        }
        [Authorize]
        public IActionResult AskAQuestion()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddQuestion(Question question, List<string> tags)
        {
            var qaR = new QARepository(_connectionString);
            var ur = new UserRepository(_connectionString);
            if (User.Identity.IsAuthenticated)
            {
                int? userId = GetCurrentUserId();
                question.UserId = userId.Value;               
                //question.User = ur.GetByEmail(User.Identity.Name);
            }
            question.DatePosted = DateTime.Now;
            qaR.AddQuestion(question, tags);
            return Redirect("/home");
        }
        public IActionResult ViewQuestion(int id)
        {
            var qaR = new QARepository(_connectionString);
            var vqvm = new ViewQuestionViewModel()
            {
                Question = qaR.GetQuestionForId(id),
                UserIsValidated = User.Identity.IsAuthenticated
            };
            return View(vqvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddAnswer(Answer answer)
        {
            if (answer == null)
            {
                return Redirect($"/home/ViewQuestion?id={answer.QuestionId}");
            }
            var qaR = new QARepository(_connectionString);
            var ur = new UserRepository(_connectionString);
            if (User.Identity.IsAuthenticated)
            {
                int? userId = GetCurrentUserId();
                answer.UserId = userId.Value;
                //answer.User = ur.GetByEmail(User.Identity.Name);
            }          
            qaR.AddAnswer(answer);
            return Redirect($"/home/ViewQuestion?id={answer.QuestionId}");
        }

        private int? GetCurrentUserId()
        {
            var ur = new UserRepository(_connectionString);
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            var user = ur.GetByEmail(User.Identity.Name);
            if (user == null)
            {
                return null;
            }

            return user.Id;
        }

    }
}