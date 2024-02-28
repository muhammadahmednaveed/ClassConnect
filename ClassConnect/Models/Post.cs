using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        [Display(Name = "Instructor")]
        public virtual int InstructorID { get; set; }

        [ForeignKey("InstructorID")]
        public virtual Instructor Instructors { get; set; }
       
        public string Author { get; set; }
        
        public DateTime Date { get; set; }

        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
    }
}
