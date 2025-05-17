 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.DATA
{
    public class Invoice : ModelBase
    {
        public Invoice()
        {
            IssuedAt = DateTime.Now;
            Sales = new List<Sale>();
        }

        public int InvoiceId { get; set; }
        public DateTime IssuedAt { get; set; }

        public string AppUserName { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public double? TotalPrice { get; set; }

        public List<Sale> Sales { get; set; }
        public bool Cancelled { get; set; } = false;

        public Order? Order { get; set; } = null;
        public int? OrderId { get; set; }
        public PaymentMethod? paymentMethod { get; set; }
    }


    public enum PaymentMethod
    {
        Cash,
        Card,
    } 
}
