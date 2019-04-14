using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReactShop.Core.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string SmallImagePath { get; set; }
        public string LargeImagePath { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal
        {
            get
            {
                return Quantity * Price;
            }
        }
    }
}