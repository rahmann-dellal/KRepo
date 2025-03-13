using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KFP.DATA
{
    public class MenuItem : ModelBase
    {
        public MenuItem()
        {

        }
        public MenuItem(string name, int price)
        {
            ItemName = name;
            SalePrice = price;
        }

        public int Id { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SalePrice { get; set; }

        public byte[]? picture { get; set; }

        public MenuItemType MenuItemType { get; set; }
        public List<Category> Categories { get; set; }
    }

    public enum MenuItemType { Main, Supliment, CanBeBoth }
}
