using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class Cart
    {
        public int Id { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
