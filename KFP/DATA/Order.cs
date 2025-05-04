using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.DATA
{
    public enum OrderStatus
    {
        Pending,
        Preparing,
        Ready,
        Completed,
        Cancelled
    }
    public enum OrderType
    {
        Table,
        Counter,
        Delivery
    }

    public class Order : ModelBase
    {
        public Order()
        {
            CreatedAt = DateTime.Now;
            OrderItems = new List<OrderItem>();
        }
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime? isPreparing { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ServedAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public string? Notes { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string AppUserName { get; set; } = string.Empty;

        public OrderType Type { get; set; } = OrderType.Counter;

        // Only used when Type == Table
        public int? TableNumber { get; set; }

        // Optional: Delivery address or phone if you want
        public DeliveryInfo DeliveryInfo { get; set; }
    }
}
