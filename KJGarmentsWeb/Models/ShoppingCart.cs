using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using WebMatrix.WebData;
namespace KJGarmentsWeb.Models
{
    public partial class ShoppingCart
    {
        MughapuEntities storeDB = new MughapuEntities();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        public string GetSessionUserName()
        {
            return ShoppingCartId;
        }
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }
        public void AddToCart(PRODUCT productdtls, int qty,string CouponCode)
        {
            // Get the matching cart and album instances
            var cartItem = storeDB.ProductCarts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.PDT_Id == productdtls.Pid);
            var userToUpdate = storeDB.PRODUCTs.SingleOrDefault(u => u.Pid == productdtls.Pid);
            if (userToUpdate != null)
            {
                userToUpdate.PDT_offer = 0;                
                userToUpdate.PDT_Discount_price = storeDB.PRODUCTs.SingleOrDefault(u => u.Pid == productdtls.Pid).PDT_Price;
                // userToUpdate.Mod_user_id = WebSecurity.CurrentUserId;
                userToUpdate.Mod_Datetime = System.DateTime.Now;

            }
            //To update offer value
            if (string.IsNullOrEmpty(CouponCode))
            {
                var offer = storeDB.Sp_GetOffer();
                var productoffer = from o in storeDB.ProductOfferValues
                                   where o.Pid == productdtls.Pid
                                   select o;


                foreach (var ofritem in productoffer)
                {
                    var updPDT = storeDB.PRODUCTs.SingleOrDefault(u => u.Pid == productdtls.Pid);
                    if (userToUpdate != null)
                    {
                        updPDT.PDT_offer = Convert.ToDecimal(ofritem.offer);
                        updPDT.offer_value_type_id = Convert.ToInt16(ofritem.OfrValtype);
                        updPDT.PDT_Discount_price = ofritem.OfrValtype == 1 ? (productdtls.PDT_Price - (ofritem.offer * productdtls.PDT_Price) / 100) : productdtls.PDT_Price - ofritem.offer;
                        // userToUpdate.Mod_user_id = WebSecurity.CurrentUserId;
                        updPDT.Mod_Datetime = System.DateTime.Now;

                    }

                }
            }
                 //To update Coupon Code value
            if (!string.IsNullOrEmpty(CouponCode))
            {
                var cupn = storeDB.Sp_GetCoupon(CouponCode);
                var productcupn = from o in storeDB.ProductCouponValues
                                   where o.Pid == productdtls.Pid
                                   select o;


                foreach (var cupnitem in productcupn)
                {
                    var updPDT = storeDB.PRODUCTs.SingleOrDefault(u => u.Pid == productdtls.Pid);
                    if (userToUpdate != null)
                    {
                        updPDT.PDT_offer = Convert.ToDecimal(cupnitem.offer);
                        updPDT.offer_value_type_id = Convert.ToInt16(cupnitem.offervaltype);
                        updPDT.PDT_Discount_price = cupnitem.offervaltype == 1 ? (productdtls.PDT_Price - (cupnitem.offer * productdtls.PDT_Price) / 100) : productdtls.PDT_Price - cupnitem.offer;
                        // userToUpdate.Mod_user_id = WebSecurity.CurrentUserId;
                        updPDT.Mod_Datetime = System.DateTime.Now;

                    }

                }
            }

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new ProductCart
                {
                    PDT_Id = productdtls.Pid,
                    CartId = ShoppingCartId,
                    Count = qty,
                    DateCreated = DateTime.Now
                };
                storeDB.ProductCarts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            storeDB.SaveChanges();
        }
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.ProductCarts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDB.ProductCarts.Remove(cartItem);
                }
                // Save changes
                storeDB.SaveChanges();
            }
            return itemCount;
        }
        public void EmptyCart()
        {
            var cartItems = storeDB.ProductCarts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.ProductCarts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
        }
        public List<ProductCart> GetCartItems()
        {
            return storeDB.ProductCarts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.ProductCarts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total


            decimal? total = (from cartItems in storeDB.ProductCarts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.PRODUCT.PDT_Discount_price).Sum();

            return total ?? decimal.Zero;
        }
        public int CreateOrder(ProductOrder order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new ProductOrderDetail
                {
                    PDT_Id = item.PDT_Id,
                    OrderId = order.OrderId,
                    UnitPrice = Convert.ToDecimal(item.PRODUCT.PDT_Discount_price),
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * Convert.ToDecimal(item.PRODUCT.PDT_Discount_price));

                storeDB.ProductOrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
           // order.Total = orderTotal;
            var orderprice = storeDB.ProductOrders.Single(oinfo => oinfo.OrderId == order.OrderId);
            orderprice.Total = orderTotal;
            
            // Save the order
            storeDB.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.OrderId;
        }
        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }
        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.ProductCarts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (ProductCart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}