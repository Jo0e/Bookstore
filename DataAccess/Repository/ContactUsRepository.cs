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
    public class ContactUsRepository : Repository<ContactUs>, IContactUsRepository
    {
        public ContactUsRepository(ApplicationDbContext context) : base(context)
        {
        }


        public ContactUs ContactDetail(int reqId)
        {
            IQueryable<ContactUs> query = dbSet;
            var contact= query.FirstOrDefault(a => a.Id == reqId);
            if (contact.CommentId > 0)
            {
                _ = query.Include(a => a.Comment).Where(a => a.Comment.Id == contact.CommentId);
            }
            
            return query.FirstOrDefault();

        }
    }
}
