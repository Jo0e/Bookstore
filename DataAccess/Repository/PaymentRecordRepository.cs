using DataAccess.DataConnection;
using DataAccess.Repository.IRepository;
using Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class PaymentRecordRepository : Repository<PaymentRecord>, IPaymentRecordRepository
    {
        public PaymentRecordRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
