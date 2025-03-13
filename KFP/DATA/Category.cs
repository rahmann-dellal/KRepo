using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFP.DATA
{
    public class Category : ModelBase
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public List<MenuItem> MenuItems { get; set; }
    }
}
