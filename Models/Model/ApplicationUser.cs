using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string ProfilePhoto { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public IList<Message>? Messages { get; set; } = [];
        public ICollection<Wishlist> Wishlists { get; set; } = [];

    }
}
