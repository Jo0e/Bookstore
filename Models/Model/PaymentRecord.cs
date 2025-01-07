using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class PaymentRecord
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public double TotalAmount { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<PurchasedItem> PurchasedItems { get; set; } = [];

    }
}
