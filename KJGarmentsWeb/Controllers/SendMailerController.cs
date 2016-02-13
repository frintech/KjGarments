using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace KJGarmentsWeb.Controllers
{
    public class SendMailerController : Controller
    {
        //
        // GET: /SendMailer/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuccessContent()
        {
            ViewBag.result = true;
            return View(); 
        }

        [HttpPost]
        public ViewResult Index(DAL.MailModel _objModelMail)
       {
           string body;
           if (ModelState.IsValid)
           {
               //using (var sr = new StreamReader(Server.MapPath("~/App_Data/Emailtemplate/") + "EmailTemplate.txt"))
               using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["EmailTEMPLATE"])))
               {
                   body = sr.ReadToEnd();
               }


               string sender = ConfigurationManager.AppSettings["EmailFromAddress"];
               string emailSubject = ConfigurationManager.AppSettings["EmailSubject"];
               string messageBody = string.Format(body, _objModelMail.Name, _objModelMail.Email, _objModelMail.Mobile.ToString(), _objModelMail.Address, _objModelMail.Comments);
               string ToEmail = ConfigurationManager.AppSettings["EmailToEmail"];
               MailMessage mail = new MailMessage();
               mail.To.Add(ToEmail);
              // mail.CC.Add(_objModelMail.Email);
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
               ViewBag.result = true;
               ModelState.Clear();
               //return View("Index", _objModelMail);
               

           }
           return View();
           
        }
    }


  
}
