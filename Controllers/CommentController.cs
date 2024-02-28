using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClassConnect.Data;
using ClassConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationContext context;
        private readonly ISession contextAccessor;

        public CommentController(ApplicationContext context, IHttpContextAccessor contextAccessor)
        {
            this.context = context;
            this.contextAccessor = contextAccessor.HttpContext.Session;
        }
        [HttpPost]
        public IActionResult AddComment(Post_Comment_Votes model)
        {
            var addComment = new Comment()
            {
                Body = model.CommentBody,
                PostID = model.PostID,
                Author = HttpContext.Session.GetString("Username"),
            };
            context.Comments.Add(addComment);
            context.SaveChanges();
            TempData["addedComment"] = "Comment Added";
            string Username = HttpContext.Session.GetString("Username");
            var instructor = context.Instructors.SingleOrDefault(e => e.Username == Username);
            var votes = context.Votes.ToList();
            if (instructor != null)
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Instructor");
            }
            else
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Student");
            }     
        }
        public IActionResult EditComment(int id)
        {
            var commentDataGet = context.Comments.SingleOrDefault(e => e.CommentID == id);
            var commentDataView = new Post_Comment_Votes()
            {
                CommentID = id,
                CommentBody = commentDataGet.Body,
                PostID = commentDataGet.PostID
            };
            return View(commentDataView);
        }
        [HttpPost]
        public IActionResult EditComment(Post_Comment_Votes model)
        {
            var EditComment = new Comment()
            {
                CommentID = model.CommentID,
                Body = model.CommentBody,
                Author = HttpContext.Session.GetString("Username"),
                PostID = model.PostID
            };
            context.Comments.Update(EditComment);
            context.SaveChanges();
            TempData["editedComment"] = "Comment edited";
            string Username = HttpContext.Session.GetString("Username");
            var instructor = context.Instructors.SingleOrDefault(e => e.Username == Username);
            var votes = context.Votes.ToList();
            if (instructor != null)
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Instructor");
            }
            else
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Student");
            }
        }
        public IActionResult DeleteComment(int id)
        {
            var comment = context.Comments.SingleOrDefault(e => e.CommentID == id);
            context.Comments.Remove(comment);
            context.SaveChanges();
            TempData["deletedComment"] = "Comment deleted";
            string Username = HttpContext.Session.GetString("Username");
            var instructor = context.Instructors.SingleOrDefault(e => e.Username == Username);
            var votes = context.Votes.ToList();
            if (instructor != null)
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Instructor");
            }
            else
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Student");
            }
        }

        public IActionResult UpVote(int id)
        {
            string Username = User.Identity.Name;
            var post = context.Posts.SingleOrDefault(e => e.PostID == id);
            var voteExist = context.Votes.SingleOrDefault(e => e.PostID == id && e.Username == Username);
            if (voteExist!=null)
            {
                if(voteExist.IsLike==true)
                {
                    context.Votes.Remove(voteExist);
                    post.Upvotes--;
                    context.Posts.Update(post);
                    context.SaveChanges();
                }
                else
                {
                    voteExist.IsLike = true;
                    voteExist.IsDislike = false;
                    context.Votes.Update(voteExist);
                    post.Upvotes++;
                    post.Downvotes--;
                    context.Posts.Update(post);
                    context.SaveChanges();
                }
            }
            else
            {
                var vote = new Vote()
                {
                    PostID = id,
                    IsLike = true,
                    IsDislike = false,
                    Username = Username
                };
                context.Votes.Add(vote);
                post.Upvotes++;
                context.Posts.Update(post);
                context.SaveChanges();
            } 
            var instructor = context.Instructors.SingleOrDefault(e => e.Username == Username);
            var votes = context.Votes.ToList();
            if (instructor != null)
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Instructor");
            }
            else
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Student");
            }
        }

        public IActionResult DownVote(int id)
        {
            string Username = User.Identity.Name;
            var post = context.Posts.SingleOrDefault(e => e.PostID == id);
            var voteExist = context.Votes.SingleOrDefault(e => e.PostID == id && e.Username == Username);
            if (voteExist != null)
            {
                if (voteExist.IsLike == false)
                {
                    context.Votes.Remove(voteExist);
                    post.Downvotes--;
                    context.Posts.Update(post);
                    context.SaveChanges();
                }
                else
                {
                    voteExist.IsLike = false;
                    voteExist.IsDislike = true;
                    context.Votes.Update(voteExist);
                    post.Upvotes--;
                    post.Downvotes++;
                    context.Posts.Update(post);
                    context.SaveChanges();
                }
            }
            else
            {
                var vote = new Vote()
                {
                    PostID = id,
                    IsLike = false,
                    IsDislike = true,
                    Username = Username
                };
                context.Votes.Add(vote);
                post.Downvotes++;
                context.Posts.Update(post);
                context.SaveChanges();
            }
            var instructor = context.Instructors.SingleOrDefault(e => e.Username == Username);
            var votes = context.Votes.ToList();
            if (instructor != null)
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Instructor");
            }
            else
            {
                ViewData["Votes"] = votes;
                return RedirectToAction("Posts", "Student");
            }
        }
    }
}
