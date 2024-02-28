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
    public class InstructorController : Controller
    {
        private readonly ApplicationContext context;
        private readonly ISession contextAccessor;

        public InstructorController(ApplicationContext context,IHttpContextAccessor contextAccessor)
        {
            this.context = context;
            this.contextAccessor = contextAccessor.HttpContext.Session;
        }

        //index is the login here.
        public IActionResult Index()
        {
            var ID= HttpContext.Session.GetString("ID");
            if(ID!=null)
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
            var obj = context.Instructors.SingleOrDefault(e => e.Username == model.Username && e.Password == model.Password);
            if (obj != null)
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, obj.Username) }, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("ID", obj.InstructorID.ToString());
                HttpContext.Session.SetString("Username", obj.Username);
                HttpContext.Session.SetString("Admin", obj.IsAdmin.ToString());
                var votes = context.Votes.ToList();
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts");
            }
            else
            {
                TempData["error"] = "Username or Password is incorrect. Maybe you are a student";
                return View(model);
            }

        }

        [AcceptVerbs("Post","Get")]
        public IActionResult UsernameExists(string Username)
        {
            var ins= context.Instructors.Where(e => e.Username == Username).SingleOrDefault();
            var std = context.Students.Where(e => e.Username == Username).SingleOrDefault();
            if (ins != null || std!=null)
            {
                return Json($"Username {Username} already in use!");
            }
            else
            {
                return Json(true);
            }
        }

        [Authorize]
        public IActionResult Signup()
        {
            var user = context.Students.SingleOrDefault(e=>e.Username==User.Identity.Name);
            if(user!=null)
            {
                return RedirectToAction("Posts","Student");
            }
            else
            {
                return View();
            }        
        }
        [Authorize]
        [HttpPost]
        public IActionResult Signup(LoginorSignUp model)
        {
            var data = new Instructor()
            {
                FullName = model.FullName,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,        
            };
            context.Instructors.Add(data);
            context.SaveChanges();
            TempData["InstructorAdded"] = "Instructor Added Successfully";
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Posts()
        {
            var posts = context.Posts.ToList();
            var postOrder = posts.OrderByDescending(t => t.Upvotes).ThenByDescending(t => t.Date);
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
        [Authorize]
        [HttpPost]
        public IActionResult Posts(Post_Comment_Votes newPost)
        {
            var Username = HttpContext.Session.GetString("Username");
            var addPost = new Post()
            {
                Title = newPost.PostTitle,
                Body = newPost.PostBody,
                InstructorID = Convert.ToInt32(HttpContext.Session.GetString("ID")),
                Author = Username,
                Date = DateTime.Now,
                Upvotes = 0,
                Downvotes = 0
            };
            context.Posts.Add(addPost);
            context.SaveChanges();
            TempData["addedPost"] = "Posted Sucessfully";
            var votes = context.Votes.ToList();
            ViewData["Votes"] = votes;
            return RedirectToAction("Posts");
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach (var cookies in storedCookies)
            {
                Response.Cookies.Delete(cookies);
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditPost(int id)
        {
            var postDataGet = context.Posts.SingleOrDefault(e => e.PostID == id);
            var postDataView = new Post()
            {
                PostID=id,
                Title = postDataGet.Title,
                Body = postDataGet.Body,
            };
            return View(postDataView);
        }
        [HttpPost]
        public IActionResult EditPost(Post model)
        {
            var RequiredPost = context.Posts.SingleOrDefault(e=>e.PostID==model.PostID);
            RequiredPost.Title = model.Title;
            RequiredPost.Body = model.Body;
            context.Posts.Update(RequiredPost);
            context.SaveChanges();
            TempData["editedPost"] = "Post edited";
            var votes = context.Votes.ToList();
            ViewData["Votes"] = votes;
            return RedirectToAction("Posts");
        }
        public IActionResult DeletePost(int id)
        {
            var post = context.Posts.SingleOrDefault(e => e.PostID == id);
            context.Posts.Remove(post);
            context.SaveChanges();
            var votes = context.Votes.ToList();
            ViewData["Votes"] = votes;
            TempData["deletedPost"] = "Post deleted";
            return RedirectToAction("Posts");
        }

    }
}
