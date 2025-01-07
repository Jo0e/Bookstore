using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public required Book Book { get; set; }
        public int Quantity { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
