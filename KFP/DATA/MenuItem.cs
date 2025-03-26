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
        public MenuItem(string name, double price, MenuItemType? type)
        {
            ItemName = name;
            SalePrice = price;
            MenuItemType = type;
        }

        public int Id { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public double SalePrice { get; set; }

        public string? pictureUri { get; set; }
        public string? thumbnailUri { get; set; }

        public MenuItemType? MenuItemType { get; set; }
        public List<Category>? Categories { get; set; }
    }

    public enum MenuItemType { Main, Addon, Universal }
}
