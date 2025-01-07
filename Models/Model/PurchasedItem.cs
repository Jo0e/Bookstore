using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class PurchasedItem
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } 
        public double BookPrice { get; set; }
        public int Quantity { get; set; } 
        public int PaymentRecordId { get; set; } 
        public PaymentRecord PaymentRecord { get; set; } 
    }
}
