using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.DATA
{
    public class DeliveryInfo
    {
        public DeliveryInfo()
        {
        }
        public int Id { get; set; }

        public string? CustomerName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
