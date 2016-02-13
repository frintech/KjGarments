using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class ApplicationCart
    {
        public string Id { get; set; } //Guid
        public string Currency { get; set; }
        public string PurchaseDescription { get; set; }
        public List<ApplicationCartItem> Items { get; set; }
        
        private decimal totalPrice { get; set; }
        public decimal TotalPrice 
        {
            get
            {
                if (Items == null)
                    return totalPrice;
                else
                    return this.Items.Sum(x => x.Quantity * x.Price);
            }
            set
            {
                // We are enabling the setting of the TotalPrice to allow for single object purchases (no cart items required)
                this.totalPrice = value;
            }
        }
    }
}