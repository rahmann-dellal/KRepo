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

    }

    public enum UserRole {  Cashier, Manager, Admin }
}
