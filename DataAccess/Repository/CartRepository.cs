using DataAccess.DataConnection;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ICollection<Cart> GetCartDetails(string userId)
        {
            return dbSet.Where(u => u.UserId == userId)
                .Include(a => a.Items).ThenInclude(z => z.Book).ToList();
        } 
    }
}
