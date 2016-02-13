using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using WebMatrix.WebData;
using System.Configuration;
using KJGarmentsWeb.Models;
using PagedList;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;


namespace KJGarmentsWeb.Controllers
{
     
    public class UserInfoController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        [Authorize]
        public ActionResult Index(int page = 1)
        {
            IEnumerable<ViewUserModel> model = null;
            model = (from p in db.User_Info                    
                     select new ViewUserModel
                     {
                         First_name = p.First_name,
                         Last_name = p.Last_name,
                         Address = p.Address_Communication,
                         Email_id = p.E_mail_id,
                         Mobile_number = p.Mobile_number,
                         Uidno = p.UidNo,
                         DOB=p.DOB,
                         Gender= p.Gender=="M" ? "Male" :"Female",
                         Isactive = p.Isactive,
                         user_type = p.Usr_type
                     }
            );

            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList().OrderByDescending(x => x.Uidno);
            return View(products.ToPagedList(page, pagesize));
        }


        [AllowAnonymous]
        public ActionResult Create()
        {
           var list = new SelectList(new[] { new { ID = "M", Name = "Male" }, new { ID = "F", Name = "FeMale" }, new { ID = "O", Name = "Others" } }, "ID", "Name", 1); 
           ViewBag.Genders = list;


           // ViewBag.Genders = new SelectList(obj, "ID", "Name", "--Select Gender--");           
            return View();
        }
        [AllowAnonymous]
        public ActionResult UserLogin(string returnUrl)
        {
            //if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
            //    returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl; 
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = TempData["PrevURL"];
            }

            return View();
            //ViewBag.ReturnUrl = returnUrl;
            //return View();
        }
        public string Encode(string value)
        {
            var hash = System.Security.Cryptography.SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
       
        public  bool ValidateUser(string username, string password)
        {
            string usrpwd=Encode(password);
            var query = from c in db.User_Info
                        where c.E_mail_id == username && c.Usr_pwd == usrpwd
                        select c;
            if (query.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private void MigrateShoppingCart(string UserName)
        {
            // Associate shopping cart items with logged-in user
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.MigrateCart(UserName);
            Session[ShoppingCart.CartSessionKey] = UserName;
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(UserLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    MigrateShoppingCart(model.UserName);

                    string decodedUrl = "";
                    if (!string.IsNullOrEmpty(returnUrl))
                        decodedUrl = Server.UrlDecode(returnUrl);
                    if (Url.IsLocalUrl(decodedUrl))
                        return Redirect(decodedUrl);
                    else
                        return RedirectToAction("Index", "Home");
                    //FormsAuthentication.RedirectFromLoginPage(model.UserName, model.RememberMe);
                   // return RedirectToAction("AddressAndPayment", "Checkout");
                }
                else
                {
                    
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(model);

           
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /User/Create
 
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(AddUserModel User)
        {
           
            //  var errors = ModelState
            //.Where(x => x.Value.Errors.Count > 0)
            //.Select(x => new { x.Key, x.Value.Errors })
            //.ToArray();
            
            var chkemailid = ChkValidEmailID(User.Email_id);
            if (chkemailid.Data != "false")
            {
                ModelState.AddModelError("Email_id" ,"This Email id has already Registered");
            }

            if (ModelState.IsValid)
            {

                try
                {
                   
                   // string usrpwd = Encode(User.Usr_pwd);
                    // Insert Product
                    User_Info objnewUser = new User_Info();
                    objnewUser.UidNo = (db.User_Info.Max(i => (int?)i.UidNo) ?? 0) + 1;  //Convert.ToInt32(db.PRODUCTs.Max(x => x.Pid)) + 1;
                    objnewUser.First_name = User.First_name;
                    objnewUser.Last_name = User.Last_name;
                    objnewUser.Isactive = true;
                    objnewUser.E_mail_id = User.Email_id;
                    objnewUser.Mobile_number = User.Mobile_number;
                    objnewUser.Address_Communication = User.Address_Communication;
                     objnewUser.DOB=User.DOB; 
                    objnewUser.Gender=User.Gender;
                    objnewUser.Usr_pwd = Encode(User.Usr_pwd);
                    db.User_Info.Add(objnewUser);
                    db.SaveChanges();
                    TempData["Usr_Message"] = ConfigurationManager.AppSettings["INS_SUC"];
                    ModelState.Clear();
                    return RedirectToAction("UserLogin", "UserInfo");

                }

                catch (Exception exception)
                {
                  
                }

                return RedirectToAction("Create", "UserInfo");

            }


            var list = new SelectList(new[] { new { ID = "M", Name = "Male" }, new { ID = "F", Name = "FeMale" }, new { ID = "O", Name = "Others" } }, "ID", "Name", 1);
            ViewBag.Genders = list;
            return View(User);
        }

       
        public ActionResult Edit(int id = 0)
        {
            var model = from p in db.User_Info
                        
                        select new EditUserModel
                        {
                            First_name = p.First_name,
                            Last_name = p.Last_name,
                            Address_Communication = p.Address_Communication,
                            Email_id = p.E_mail_id,
                            Mobile_number = p.Mobile_number,
                            Uidno = p.UidNo,                           
                            user_type = p.Usr_type,
                            DOB=p.DOB,
                            Gender=p.Gender

                        };
            EditUserModel editprddetails = null;
            foreach (var m in model)
            {
                editprddetails = new EditUserModel();
                editprddetails.Uidno = m.Uidno;
                editprddetails.First_name = m.First_name;
                editprddetails.Last_name = m.Last_name;
                editprddetails.Address_Communication = m.Address_Communication;
                editprddetails.Email_id = m.Email_id;
                editprddetails.Mobile_number = m.Mobile_number;
                   editprddetails.DOB = m.DOB;
                   editprddetails.Gender = m.Gender;
                   editprddetails.user_type = m.user_type;
            }

            var list = new SelectList(new[] { new { ID = "M", Name = "Male" }, new { ID = "F", Name = "FeMale" }, new { ID = "O", Name = "Others" } }, "ID", "Name", 1);
            ViewBag.Genders = list;  
         

            return View(editprddetails);
        }

        //
        // POST: /Brand/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserModel user)
        {
          //  var errors = ModelState
          //.Where(x => x.Value.Errors.Count > 0)
          //.Select(x => new { x.Key, x.Value.Errors })
          //.ToArray();
            if (ModelState.IsValid)
            {
                var userToUpdatebrands = db.User_Info.SingleOrDefault(u => u.UidNo == user.Uidno);
                if (userToUpdatebrands != null)
                {
                    userToUpdatebrands.First_name = user.First_name;

                    userToUpdatebrands.Last_name = user.Last_name;
                    userToUpdatebrands.E_mail_id = user.Email_id;
                    userToUpdatebrands.Mobile_number = user.Mobile_number;
                    userToUpdatebrands.Address_Communication = user.Address_Communication;
                    userToUpdatebrands.DOB = user.DOB;
                    userToUpdatebrands.Gender = user.Gender;
                    userToUpdatebrands.Usr_type = user.user_type;
                    db.SaveChanges();
                }
                TempData["Usr_Message"] = ConfigurationManager.AppSettings["EDT_SUC"];
                return RedirectToAction("Index");

            }
            return View(user);
        }


        [Authorize]
        public ActionResult Delete(int id = 0)
        {

            var model = from p in db.User_Info                       
                        where p.UidNo == id
                        select new deleteUserModel
                        {
                            uidNo = p.UidNo,
                            First_name = p.First_name,
                            Last_name = p.Last_name,


                        };
            deleteUserModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteUserModel();
                delbrnadmodel.uidNo = m.uidNo;
                delbrnadmodel.First_name = m.First_name;
                delbrnadmodel.Last_name = m.Last_name;

            }

            return View(delbrnadmodel);
        }
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var userToUpdate = db.User_Info.SingleOrDefault(u => u.UidNo == id);
            if (userToUpdate != null)
            {

                userToUpdate.Isactive = false;
               
                db.SaveChanges();
            }
            TempData["Usr_Message"] = ConfigurationManager.AppSettings["DEL_SUC"];
            return RedirectToAction("Index");
        }
         [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = from p in db.User_Info
                        where p.UidNo == id
                        select new deleteUserModel
                        {
                            uidNo = p.UidNo,
                            First_name = p.First_name,
                            Last_name = p.Last_name,


                        };
            deleteUserModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteUserModel();
                delbrnadmodel.uidNo = m.uidNo;
                delbrnadmodel.First_name = m.First_name;
                delbrnadmodel.Last_name = m.Last_name;

            }

            return View(delbrnadmodel);
        }

        //
        // POST: /Brand/Delete/5
        [Authorize]
        [HttpPost, ActionName("ReActivate")]
        public ActionResult ReActivateConfirmed(int id)
        {
            var userToUpdate = db.User_Info.SingleOrDefault(u => u.UidNo == id);
            if (userToUpdate != null)
            {

                userToUpdate.Isactive = true;
    

                db.SaveChanges();
            }
            TempData["Usr_Message"] = ConfigurationManager.AppSettings["RAV_SUC"];
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            //ViewBag.StatusMessage =
            //    message == KJGarmentsWeb.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            //    : message == KJGarmentsWeb.Controllers.AccountController.ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            //    : message == KJGarmentsWeb.Controllers.AccountController.ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            //    : "";
            //ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            //ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            bool hasLocalAccount = false;
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                hasLocalAccount = true;
            }

            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ResetPassword(User.Identity.Name, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = KJGarmentsWeb.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {


                if (ModelState.IsValid)
                {
                    try
                    {
                        
                        var userToUpdate = db.User_Info.Where(u => u.E_mail_id == model.userName).Where(u => u.Mobile_number == model.MobileNo).SingleOrDefault();
                        if (userToUpdate != null)
                        {

                            userToUpdate.Usr_pwd = Encode(model.NewPassword);

                            db.SaveChanges();
                        }
                        TempData["CHNG_PWD"] = ConfigurationManager.AppSettings["RAV_SUC"];
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //
        // POST: /Account/Manage
         [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserPasswordModel model)
        {
            bool hasLocalAccount = false;
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                hasLocalAccount = true;
            }
           
            //if (hasLocalAccount)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        // ChangePassword will throw an exception rather than return false in certain failure scenarios.
            //        bool changePasswordSucceeded;
            //        try
            //        {
            //            string oldpassword = Encode(model.OldPassword);
            //            string NewPassword = Encode(model.NewPassword);
            //            changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, oldpassword, NewPassword);
            //        }
            //        catch (Exception ex)
            //        {
            //            changePasswordSucceeded = false;
            //        }

            //        if (changePasswordSucceeded)
            //        {
            //            return RedirectToAction("Manage", new { Message = KJGarmentsWeb.Controllers.AccountController.ManageMessageId.ChangePasswordSuccess });
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            //        }
            //    }
            //}
            //else
            //{
                

                if (ModelState.IsValid)
                {
                    try
                    {
                        string oldpassword = Encode(model.OldPassword);
                        var userToUpdate = db.User_Info.Where(u => u.E_mail_id == model.userName).Where(u => u.Usr_pwd == oldpassword).SingleOrDefault();
                        if (userToUpdate != null)
                        {

                            userToUpdate.Usr_pwd = Encode(model.NewPassword);                          

                            db.SaveChanges();
                        }
                        TempData["CHNG_PWD"] = ConfigurationManager.AppSettings["RAV_SUC"];
                        return RedirectToAction("Index","Home");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            //}

            // If we got this far, something failed, redisplay form
            return View(model);
        }
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult LogOff()
         {
             WebSecurity.Logout();
             var cart = ShoppingCart.GetCart(this.HttpContext);
             cart.EmptyCart();
            

             return RedirectToAction("Index", "Home");
         }

         public JsonResult ChkValidEmailID(string emailid)
         {


             var chkemailidexists = from a in db.User_Info
                                  where a.Isactive == true && (a.E_mail_id == emailid)
                                  select a;
             foreach (var d in chkemailidexists)
             {
                 return Json("true");
             }
             return Json("false");



         }
    }
}
