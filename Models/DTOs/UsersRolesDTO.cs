using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UsersRolesDTO
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
