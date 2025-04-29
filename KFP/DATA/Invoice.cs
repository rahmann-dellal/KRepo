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
            InvoiceDate = DateTime.Now;
            Sales = new List<Sale>();
        }

        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string AppUserName { get; set; }
        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int TotalPrice { get; set; }

        public List<Sale> Sales { get; set; }
        public bool Cancelled { get; set; } = false;

        public Order? Order { get; set; } = null;
    }
}
