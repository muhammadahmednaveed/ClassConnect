using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }

        [Display(Name = "Post")]
        public virtual int PostID { get; set; }

        [ForeignKey("PostID")]
        public virtual Post Posts { get; set; }

    }
}
