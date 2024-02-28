using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClassConnect.Data;
using ClassConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassConnect.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationContext context;
        private readonly ISession contextAccessor;

        public StudentController(ApplicationContext context, IHttpContextAccessor contextAccessor)
        {
            this.context = context;
            this.contextAccessor = contextAccessor.HttpContext.Session;
        }

        //index is the login here.
        public IActionResult Index()
        {
            var ID = HttpContext.Session.GetString("StudentID");
            if (ID != null)
            {
                var votes = context.Votes.ToList();
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Index(LoginorSignUp model)
        {
            var obj = context.Students.SingleOrDefault(e => e.Username == model.Username && e.Password == model.Password);
            if (obj != null)
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, obj.Username) }, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("StudentID", obj.StudentID.ToString());
                HttpContext.Session.SetString("Username", model.Username);
                var votes = context.Votes.ToList();
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts");
            }
            else
            {
                TempData["error"] = "Username or Password is incorrect. Maybe You are an instructor";
                return View(model);
            }

        }

        [AcceptVerbs("Post","Get")]
        public IActionResult UsernameExists(string Username)
        {
            var ins = context.Instructors.Where(e => e.Username == Username).SingleOrDefault();
            var std = context.Students.Where(e => e.Username == Username).SingleOrDefault();
            if (ins != null || std != null)
            {
                return Json($"Username {Username} already in use!");
            }
            else
            {
                return Json(true);
            }
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Signup(LoginorSignUp model)
        {

            var data = new Student()
            {
                FullName = model.FullName,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
            };
            context.Students.Add(data);
            context.SaveChanges();
            TempData["success"] = "You have successfully created the account please login";
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Posts()
        {
            var posts = context.Posts.ToList();
            var postOrder = posts.OrderByDescending(t => t.Upvotes).ThenByDescending(t=>t.Date);
            ViewData["Posts"] = postOrder;
            var commentList = context.Comments.ToList();
            Dictionary<int, List<Comment>> CommentDict = new Dictionary<int, List<Comment>>();
            foreach (Comment comment in commentList)
            {
                if (CommentDict.ContainsKey(comment.PostID))
                {
                    CommentDict[comment.PostID].Add(comment);
                }
                else
                {
                    List<Comment> comments = new List<Comment>();
                    comments.Add(comment);
                    CommentDict.Add(comment.PostID, comments);
                }
            }
            ViewData["Comments"] = CommentDict;
            var votes = context.Votes.ToList();
            ViewData["Votes"] = votes;
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach(var cookies in storedCookies)
            {
                Response.Cookies.Delete(cookies);
            }
            return RedirectToAction("Index");
        }

    }
}
