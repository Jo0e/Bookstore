using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public required string CommentString { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsEdited { get; set; } = false;
        public int Likes { get; set; } = 0;
        public IList<string> ReactionUsersId { get; set; } = [];
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
