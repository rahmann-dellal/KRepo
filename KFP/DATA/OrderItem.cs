using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public int? OrderId { get; set; }
        public Order? Order { get; set; } = null!;

        public int? MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; } = null!;
        public string MenuItemName { get; set; } = string.Empty;

        [NotMapped]
        private int _quantity = 1;
        public int Quantity { 
            get
            {
                return _quantity;
            } 
            set
            {
                if (value < 1)
                {
                    _quantity = 1;
                }
                else
                {
                    _quantity = value;
                }
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            } 
        }
        public double UnitPrice { get; set; }

        [NotMapped]
        public double TotalPrice => UnitPrice * Quantity;

        public int? ParentOrderItemId { get; set; }
        public OrderItem? ParentOrderItem { get; set; }

        public List<OrderItem> AddOns { get; set; } = new();

        [NotMapped]
        public bool IsAddOn => ParentOrderItem != null;
    }

}
