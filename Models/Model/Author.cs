using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ProfilePhoto { get; set; }
        public ICollection<Book> Books { get; set; } = [];

    }
}
