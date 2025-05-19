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
    public enum OrderLocation
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

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? SetPreparingAt { get; set; }
        public DateTime? SetReadyAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public string? Notes { get; set; }
        public Session? Session { get; set; }
        public int? SessionId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public int? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public string AppUserName { get; set; } = string.Empty;

        public OrderLocation orderLocation { get; set; } = OrderLocation.Counter;

        // Only used when Type == Table
        public int? TableNumber { get; set; }

        // Optional: Delivery address or phone if you want
        public DeliveryInfo DeliveryInfo { get; set; }

        public double? TotalPrice { get; set; }
        public Invoice? Invoice { get; set; } = null!;
        public PaymentMethod? paymentMethod { get; set; }
    }
}
