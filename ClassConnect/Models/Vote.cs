using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Models
{
    public class Vote
    {
        [Key]
        public int VoteID { get; set; }
        public bool IsLike { get; set; }
        public bool IsDislike { get; set; }
        
        [Display(Name = "Post")]
        public virtual int PostID { get; set; }

        [ForeignKey("PostID")]
        public virtual Post Posts { get; set; }
        public string Username { get; set; }



    }
}
