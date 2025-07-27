using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFP.DATA
{
    public class AppUser : ModelBase
    {
        public int AppUserID { get; set; }
        
        [Required]
        [StringLength(20)]
        [MinLength(3)]
        public string UserName { get; set; }
        
        [Required]
        public string PINHash { get; set; }
        
        [Required]
        public UserRole Role { get; set; }

        public int avatarCode { get; set; }

        public Boolean HasPrivelegesOf(UserRole role)
        {
            if (this.Role >= role) return true;
            else return false;
        }
        public List<PaymentReceipt> Receipts { get; set; } = new();
        public List<Order> Orders { get; set; } = new();

        public List<Session> Sessions { get; set; } = new();
    }

    public enum UserRole {  Cashier, Manager, Admin }
}
