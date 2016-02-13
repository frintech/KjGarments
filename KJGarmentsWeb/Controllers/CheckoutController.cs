using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using System.IO;
using System.Configuration;
using System.Net.Mail;
namespace KJGarmentsWeb.Controllers
{
 // [Authorize]
    public class CheckoutController : Controller
    {
        //
        // GET: /Checkout/
        MughapuEntities storeDB = new MughapuEntities();
       public string PromoCode = System.Configuration.ConfigurationManager.AppSettings["PROMOCODE"];//"FREE";
       
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult AddressAndPayment()
        {
            ProductOrder objprdorder = new ProductOrder();
            if (Request.IsAuthenticated)
            {
                var query = from c in storeDB.User_Info
                            where c.E_mail_id == User.Identity.Name
                            select c;
                foreach (var d in query)
                {
                   
                    objprdorder.Customer_name = d.First_name + d.Last_name;
                    objprdorder.mobileNo = d.Mobile_number;
                    objprdorder.Emailid = d.E_mail_id;
                    objprdorder.Customer_Address = d.Address_Communication;
                }

            }
            else
            {
                TempData["PrevURL"] = "/Checkout/AddressAndPayment";
                //return RedirectToAction("UserLogin", "UserInfo");
            }
            return View(objprdorder);
           
        }
      
        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new ProductOrder();
            TryUpdateModel(order);

            try
            {

                //if ((PromoCode.ToUpper().Split(',').Contains(values["PromoCode"].ToUpper()) == false) && (string.IsNullOrEmpty(values["PromoCode"]) == false))
                //{
                //    ViewBag.Message =  "Invalid Promo Code.";
                //    return View(order);
                //}
                //else
                //{
                   // order.Username = User.Identity.Name;
                   order.OrderDate = DateTime.Now;
                    //order.Currency = System.Configuration.ConfigurationManager.AppSettings["currency_code"];
                   order.Currency = System.Configuration.ConfigurationManager.AppSettings["currency_code"];
                    //order.Name = values["PromoCode"];
                    //Save Order
                    storeDB.ProductOrders.Add(order);
                    storeDB.SaveChanges();
                    //Process the order
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    cart.CreateOrder(order);
                    if (!string.IsNullOrEmpty(order.Customer_name))
                    {
                        SendEmail(order.Customer_name, order.Customer_Address, order.Emailid, order.OrderId.ToString(), order.mobileNo);
                    }
                    return RedirectToAction("Complete",
                        new { id = order.OrderId });
                //}
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }
        
        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.ProductOrders.Any(
                o => o.OrderId == id);// && o.Username == User.Identity.Name

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
        public void SendEmail(string name, string addressinfo,string emailid,string orderid,string mobileno)
        {
            string body=string.Empty;
            string strprice = string.Empty;
            string strtitle = string.Empty;
            string strqty = string.Empty;
            string strUnitprice = string.Empty;
            string strorderdate = string.Empty;
            string strItems = string.Empty;
            //using (var sr = new StreamReader(Server.MapPath("~/App_Data/Emailtemplate/") + "EmailTemplate.txt"))
            using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["ConfirmationEmail"])))
            {
                body = sr.ReadToEnd();
            }



            int oid = int.Parse(orderid);
            strorderdate = storeDB.ProductOrders.Where(x => x.OrderId == oid).FirstOrDefault().OrderDate.ToString("dd/MM/yyyy");
            //string strprice = storeDB.OrderDetails.Where(x => x.OrderId == oid).FirstOrDefault().UnitPrice.ToString();
            //int imageid = storeDB.OrderDetails.Where(x => x.OrderId == oid).FirstOrDefault().ImageId;
            //string strtitle = storeDB.ImageDetails.Where(x => x.ImageId == imageid).FirstOrDefault().ImageDescription;
            var catlist = from I in storeDB.ProductOrderDetails
                          join G in storeDB.PRODUCTs on I.PDT_Id equals G.Pid
                      where I.OrderId == oid
                      select new{I.UnitPrice,G.PDT_Name,I.Quantity};
           
            foreach (var i in catlist)
            {
                strqty = i.Quantity.ToString();
                strUnitprice = i.UnitPrice.ToString();
                strprice = (i.Quantity * i.UnitPrice).ToString();
                strtitle = i.PDT_Name.ToString();
                strItems += @"<table border ='1'>
   <tr><td>Tracking Reference Id:</td><td>"+orderid+@"</td></tr>
   <tr><td>Purchase Item  Description:</td><td>"+strtitle+@"</td></tr>
   <tr><td>Price(INR):</td><td>"+strUnitprice+@"</td></tr>
   <tr><td>Qty:</td><td>"+strqty+@"</td></tr>
   <tr><td>Total Amount(INR):</td><td>"+strprice+@"</td></tr>
   <tr><td>Order Date:</td><td>"+strorderdate+@"</td></tr>
   </table> <br><br>";


            }

            // Product Owner email
           
            string sender = ConfigurationManager.AppSettings["EmailFromAddress"];
            string emailSubject = ConfigurationManager.AppSettings["ConfirmationEmailSubject"];
            string messageBody = string.Format(body, orderid, strtitle, strprice, name, emailid, mobileno, addressinfo, strUnitprice, strqty, strorderdate);
            string ToEmail = emailid;//ConfigurationManager.AppSettings["EmailToEmail"];
            MailMessage mail = new MailMessage();
            mail.To.Add(ConfigurationManager.AppSettings["EmailToEmail"]);
           // mail.CC.Add(ConfigurationManager.AppSettings["EmailToEmail"]);
            mail.From = new MailAddress(sender);
            mail.Subject = emailSubject;
            mail.Body = messageBody;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["EmailHost"];
            smtp.Port = Convert.ToInt16(ConfigurationManager.AppSettings["EmailPort"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            (ConfigurationManager.AppSettings["EmailUid"], ConfigurationManager.AppSettings["EmailPwd"]);// Enter seders User name and password
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]); ;
            smtp.Send(mail);

            // Customer  copy email

            using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["EndusermailTEMPLATE"])))
            {
                body = sr.ReadToEnd();
            }



            messageBody = string.Format(body, orderid, strtitle, strprice, name, strorderdate, strUnitprice, strqty, strItems);
            
            MailMessage mailcust = new MailMessage();
            mailcust.To.Add(emailid);
            //mailcust.CC.Add(ConfigurationManager.AppSettings["EmailToEmail"]);
            mailcust.From = new MailAddress(sender);
            mailcust.Subject = emailSubject;
            mailcust.Body = messageBody;
            mailcust.IsBodyHtml = true;
            SmtpClient smtpcust = new SmtpClient();
            smtpcust.Host = ConfigurationManager.AppSettings["EmailHost"];
            smtpcust.Port = Convert.ToInt16(ConfigurationManager.AppSettings["EmailPort"]);
            smtpcust.UseDefaultCredentials = false;
            smtpcust.Credentials = new System.Net.NetworkCredential
            (ConfigurationManager.AppSettings["EmailUid"], ConfigurationManager.AppSettings["EmailPwd"]);// Enter seders User name and password
            smtpcust.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]); ;
            smtpcust.Send(mailcust);

        }

    }
}
