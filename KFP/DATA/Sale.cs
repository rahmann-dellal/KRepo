using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.DATA
{
    public class Sale : ModelBase
    {
        public Sale()
        {
        }

        public Sale(string itemName, int unitPrice, int quantity)
        {
            ItemName = itemName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public int SaleId { get; set; }
        public string ItemName { get; set; }
        public int? MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }

        [NotMapped]
        public int TotalPrice => UnitPrice * Quantity;

        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public bool Cancelled { get; set; } = false;
    }
}
