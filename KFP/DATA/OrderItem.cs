using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace KFP.DATA
{
    public class OrderItem : ModelBase
    {
        public OrderItem()
        {
        }
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int? MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; } = null!;
        public string MenuItemName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        [NotMapped]
        public double TotalPrice => UnitPrice * Quantity;

        public int? ParentOrderItemId { get; set; }
        public OrderItem? ParentOrderItem { get; set; }

        public List<OrderItem> AddOns { get; set; }
        public List<OrderItem> ChildrenOrderItems { get; set; } 
    }

}
