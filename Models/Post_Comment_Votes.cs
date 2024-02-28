using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassConnect.Models
{
    public class Post_Comment_Votes
    {
        public string Username { get; set; }
        public int PostID { get; set; }
        public int CommentID { get; set; }
        public string PostTitle { get; set; }

        public string PostBody { get; set; }
        public string CommentBody { get; set; }

        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public bool IsLike { get; set; }
        public bool IsDislike { get; set; }
    }
}
