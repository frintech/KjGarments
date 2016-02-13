using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class ApplicationCartItem
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}