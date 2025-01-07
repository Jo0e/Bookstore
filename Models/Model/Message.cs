using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Message
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string MessageString { get; set; }
        public DateTime MessageDateTime { get; set; }
        public bool IsReadied { get; set; } = false;

        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
