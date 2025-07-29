using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ShoppingCart
    {
        public string Id { get; set; }
        public List<CartItem> Items { get; set; } = [];
    }
}