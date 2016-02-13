using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using WebMatrix.WebData;
using System.Configuration;
using PagedList;

namespace KJGarmentsWeb.Controllers
{
    public class CouponController : Controller
    {
        private MughapuEntities db = new MughapuEntities();
        //
        // GET: /Offer/
        [Authorize]
        public ActionResult Index(int page = 1)
        {
            IEnumerable<ViewCouponModel> model = null;
            model = (from p in db.Coupon_Code
                     join u in db.User_Info on p.user_id equals u.UidNo
                     where p.Active==true
                     select new ViewCouponModel
                     {

                         cupn_Code = p.Cupn_Code,
                         cupn_Name = p.Cupn_Name,
                         Description = p.Description,                      
                         StartDate = p.StartDate,
                         EndDate = p.EndDate,
                         MinQty = p.MinQty,
                         MaxQty = p.MaxQty,
                         Isactive = p.Active,
                         couponid = p.Cupn_Id,
                         usercode=u.First_name + "," +u.Last_name

                     }
            );

            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList().OrderByDescending(x => x.couponid);
            return View(products.ToPagedList(page, pagesize));
        }
        private void SetChildren(CouponModel model, List<CouponModel> userList)
        {
            var childs = userList.
                            Where(x => x.parent_name == model.Item_Name).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    SetChildren(child, userList);
                    model.tvlists.Add(child);
                }
            }
        }
        private void EditSetChildren(EditCouponModel model, List<EditCouponModel> userList, int Couponid)
        {
            var childs = userList.
                            Where(x => x.parent_name == model.Item_Name).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    EditSetChildren(child, userList, Couponid);
                    model.tvlists.Add(child);
                }
            }
        }
        //
        // GET: /Offer/Details/5

       
        //
        // GET: /Offer/Create
        [Authorize]
        public ActionResult Create()
        {
            var treeview = db.GetTreeView();
            List<CouponModel> userList;

            userList = (from t in db.TreeviewLists
                        select new CouponModel
                        {
                            id = t.id,
                            menu_id = t.menu_id,
                            Parent_Menuid = t.Parent_Menuid,
                            origin = t.origin,
                            Item_Name = t.Item_Name,
                            parent_name = t.parent_name
                        }).ToList();

            var president = userList.
                              Where(x => x.parent_name == "XYA").FirstOrDefault();
            SetChildren(president, userList);
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            //ViewBag.users = new SelectList(db.User_Info, "UserId", "firs");

            var stands = db.User_Info.ToList().Select(s=> new {ID=s.UidNo,Name=string.Format("{0}, {1}", s.First_name, s.Last_name)});

            ViewBag.users = new SelectList(stands, "ID", "Name");


            president.cupn_Code = GetVoucherNumber();

            return View(president);
        }

        private Random random = new Random();

        public string GetVoucherNumber()
        {
            int length = 7;
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            if (db.Coupon_Code.Where(x => x.Cupn_Code == result).Any())
            {
                GetVoucherNumber();
            }
            return result;
        }
        //
        // POST: /Offer/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(CouponModel Coupon)
        {
            string strPdtNames = Request["PDT_Names"];
            string strPdtdiscnt = Request["PDT_Discnt"];
            string strPdtofrtype = Request["PDT_OFRTYPE"];


            //  var errors = ModelState
            //.Where(x => x.Value.Errors.Count > 0)
            //.Select(x => new { x.Key, x.Value.Errors })
            //.ToArray();
            if (ModelState.IsValid)
            {
                try
                {

                    // Insert Product
                    int Cupn_Id = (db.Coupon_Code.Max(i => (int?)i.Cupn_Id) ?? 0) + 1;
                    Coupon_Code objnewCupn = new Coupon_Code();
                    // objnewCupn.Cupn_Id = Cupn_Id;
                    objnewCupn.Cupn_Code = Coupon.cupn_Code;
                    objnewCupn.Cupn_Name = Coupon.cupn_Name;
                    objnewCupn.Description = Coupon.Description;                   
                    objnewCupn.StartDate = Coupon.StartDate;
                    objnewCupn.EndDate = Coupon.EndDate;
                    objnewCupn.MinQty = Convert.ToInt32(Coupon.MinQty);
                    objnewCupn.MaxQty = Convert.ToInt32(Coupon.MaxQty);                 
                    objnewCupn.Active = true;
                    objnewCupn.user_id = Coupon.userid;
                    objnewCupn.Adduser = WebSecurity.CurrentUserId;
                    objnewCupn.ModifiedUser = WebSecurity.CurrentUserId;
                    objnewCupn.AddDatetime = System.DateTime.Now;
                    objnewCupn.ModifiedDate = System.DateTime.Now;
                    db.Coupon_Code.Add(objnewCupn);
                    db.SaveChanges();

                    foreach (var m in strPdtNames.Split('{'))
                    {

                        if (m != "[")
                        {
                            if (!string.IsNullOrEmpty(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", "")))
                            {
                                CouponProduct objcupnPdt = new CouponProduct();
                                objcupnPdt.cupn_No = (db.CouponProducts.Max(i => (int?)i.cupn_No) ?? 0) + 1;
                                objcupnPdt.CupnID = Cupn_Id;
                                objcupnPdt.DiscountPct = Convert.ToInt32(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.OFR_VAL_TYPE = Convert.ToInt32(m.Split(',')[2].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.ProductID = Convert.ToInt32(m.Split(',')[3].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.PDT_TYPE = m.Split(',')[4].Split(':')[1].ToString().Replace("\"", "").Replace("}", "").Replace("]", "");
                                objcupnPdt.Adduser = WebSecurity.CurrentUserId;
                                objcupnPdt.ModifiedUser = WebSecurity.CurrentUserId;
                                objcupnPdt.AddDatetime = System.DateTime.Now;
                                objcupnPdt.ModifiedDate = System.DateTime.Now;
                                db.CouponProducts.Add(objcupnPdt);
                                db.SaveChanges();
                            }
                        }
                    }
                    TempData["Coupon_Message"] = ConfigurationManager.AppSettings["INS_SUC"];
                    ModelState.Clear();
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    var stands = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
                    ViewBag.users = new SelectList(stands, "ID", "Name");
                    return RedirectToAction("Create", "Coupon");
                }
                catch
                {
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    var stands = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
                    ViewBag.users = new SelectList(stands, "ID", "Name");
                    return View();
                }
            }
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            var stands1 = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
            ViewBag.users = new SelectList(stands1, "ID", "Name");
            return View();
        }

        //
        // GET: /Offer/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {

            EditCouponModel editCoupondetails = new EditCouponModel();
            var treeview = db.GetTreeView();
            List<EditCouponModel> userList=new List<EditCouponModel>();

            //userList = (from t in db.TreeviewLists
            //            join S in db.CouponProducts.Where(x => x.CupnID == id) on new { a = t.menu_id, b = t.origin } equals new { a = S.ProductID, b = S.PDT_TYPE }
            //            into tvGroup
            //            from p in tvGroup.DefaultIfEmpty()
            //            //where itemOffer.Cupn_Id == id
            //            select new EditCouponModel
            //            {
            //                id = t.id,
            //                menu_id = t.menu_id,
            //                Parent_Menuid = t.Parent_Menuid,
            //                origin = t.origin,
            //                Item_Name = t.Item_Name,
            //                parent_name = t.parent_name,
            //                Discountpct = (p.DiscountPct == null ? null : p.DiscountPct),
            //                offer_id= (p.OFR_VAL_TYPE == null ? null : p.OFR_VAL_TYPE)

            //            }).ToList();

            editCoupondetails = userList.
                              Where(x => x.parent_name == "XYA").FirstOrDefault();
            EditSetChildren(editCoupondetails, userList, id);
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
          

            var model = from p in db.Coupon_Code
                        where p.Cupn_Id == id
                        select new
                        {
                            p.Cupn_Code,
                            p.Cupn_Name,
                            p.Cupn_Id,                          
                            p.MinQty,
                            p.MaxQty,
                            p.StartDate,
                            p.EndDate,
                            p.user_id,
                            p.Description
                        };


            foreach (var m in model)
            {
                // editCoupondetails = new EditCouponModel();
                editCoupondetails.Coupon_Code = m.Cupn_Code;
                editCoupondetails.Coupon_Name = m.Cupn_Name;
                editCoupondetails.Couponid = m.Cupn_Id;              
                editCoupondetails.MinQty = m.MinQty;
                editCoupondetails.MaxQty = m.MaxQty;
                editCoupondetails.StartDate = m.StartDate;
                editCoupondetails.EndDate = m.EndDate;
                editCoupondetails.userid = m.user_id;
                editCoupondetails.Description = m.Description;
                    
            }



            var stands = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
            ViewBag.users = new SelectList(stands, "ID", "Name", editCoupondetails.userid);

            return View(editCoupondetails);
        }

        //
        // POST: /Offer/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(EditCouponModel edtCoupon)
        {
            string strPdtNames = Request["PDT_Names"];
            string strPdtdiscnt = Request["PDT_Discnt"];
            string strPdtofrtype = Request["PDT_OFRTYPE"];
            string ImageName, physicalPath, targetPath;

            //  var errors = ModelState
            //.Where(x => x.Value.Errors.Count > 0)
            //.Select(x => new { x.Key, x.Value.Errors })
            //.ToArray();
            if (ModelState.IsValid)
            {
                try
                {

                    var userToUpdateproducts = db.Coupon_Code.SingleOrDefault(u => u.Cupn_Id == edtCoupon.Couponid);
                    if (userToUpdateproducts != null)
                    {
                        // userToUpdateproducts.Coupon_Code = edtCoupon.Coupon_Code;
                        userToUpdateproducts.Cupn_Name = edtCoupon.Coupon_Name;
                        userToUpdateproducts.Description = edtCoupon.Description;
                        userToUpdateproducts.MinQty = Convert.ToInt32(edtCoupon.MinQty);
                        userToUpdateproducts.MaxQty = Convert.ToInt32(edtCoupon.MaxQty);
                        userToUpdateproducts.StartDate = edtCoupon.StartDate;
                        userToUpdateproducts.EndDate = edtCoupon.EndDate;
                        userToUpdateproducts.ModifiedUser = WebSecurity.CurrentUserId;
                        userToUpdateproducts.ModifiedDate = System.DateTime.Now;
                        userToUpdateproducts.user_id = edtCoupon.userid;
                        db.SaveChanges();
                    }
                    IList<CouponProduct> tvItem = (from p in db.CouponProducts
                                                         where p.CupnID == edtCoupon.Couponid
                                                         select p).ToList();
                    foreach (CouponProduct s in tvItem)
                    {

                        db.CouponProducts.Remove(s);

                    }
                    db.SaveChanges();


                    foreach (var m in strPdtNames.Split('{'))
                    {

                        if (m != "[")
                        {
                            if (!string.IsNullOrEmpty(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", "")))
                            {
                                CouponProduct objcupnPdt = new CouponProduct();
                                objcupnPdt.cupn_No = (db.CouponProducts.Max(i => (int?)i.cupn_No) ?? 0) + 1;
                                objcupnPdt.CupnID = edtCoupon.Couponid;
                                objcupnPdt.DiscountPct = Convert.ToInt32(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.OFR_VAL_TYPE = Convert.ToInt32(m.Split(',')[2].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.ProductID = Convert.ToInt32(m.Split(',')[3].Split(':')[1].ToString().Replace("\"", ""));
                                objcupnPdt.PDT_TYPE = m.Split(',')[4].Split(':')[1].ToString().Replace("\"", "").Replace("}", "").Replace("]", "");
                                objcupnPdt.Adduser = WebSecurity.CurrentUserId;
                                objcupnPdt.ModifiedUser = WebSecurity.CurrentUserId;
                                objcupnPdt.AddDatetime = System.DateTime.Now;
                                objcupnPdt.ModifiedDate = System.DateTime.Now;
                                db.CouponProducts.Add(objcupnPdt);
                                db.SaveChanges();
                            }
                        }
                    }
                    TempData["Coupon_Message"] = ConfigurationManager.AppSettings["EDT_SUC"];
                    return RedirectToAction("Index");




                }
                catch
                {
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    var stands = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
                    ViewBag.users = new SelectList(stands, "ID", "Name");
                    return View();
                }
            }
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            var stands1 = db.User_Info.ToList().Select(s => new { ID = s.UidNo, Name = string.Format("{0}, {1}", s.First_name, s.Last_name) });
            ViewBag.users = new SelectList(stands1, "ID", "Name");
            return View();
        }

        //
        // GET: /Offer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var model = from p in db.Coupon_Code
                        where p.Cupn_Id == id
                        select new deleteCouponModel
                        {
                            couponid = p.Cupn_Id,
                            cupn_Code = p.Cupn_Code,
                            cupn_Name = p.Cupn_Name,


                        };

            deleteCouponModel delCouponmodel = null;
            foreach (var m in model)
            {
                delCouponmodel = new deleteCouponModel();
                delCouponmodel.couponid = m.couponid;
                delCouponmodel.cupn_Code = m.cupn_Code;
                delCouponmodel.cupn_Name = m.cupn_Name;

            }

            return View(delCouponmodel);

        }

        //
        // POST: /Offer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var userToUpdate = db.Coupon_Code.SingleOrDefault(u => u.Cupn_Id == id);
            if (userToUpdate != null)
            {

                userToUpdate.Active = false;
                userToUpdate.ModifiedUser = WebSecurity.CurrentUserId;
                userToUpdate.ModifiedDate = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Coupon_Message"] = ConfigurationManager.AppSettings["DEL_SUC"];
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = from p in db.Coupon_Code
                        where p.Cupn_Id == id
                        select new deleteCouponModel
                        {
                            couponid = p.Cupn_Id,
                            cupn_Code = p.Cupn_Code,
                            cupn_Name = p.Cupn_Name,


                        };

            deleteCouponModel delCouponmodel = null;
            foreach (var m in model)
            {
                delCouponmodel = new deleteCouponModel();
                delCouponmodel.couponid = m.couponid;
                delCouponmodel.cupn_Code = m.cupn_Code;
                delCouponmodel.cupn_Name = m.cupn_Name;

            }

            return View(delCouponmodel);
        }

        //
        // POST: /Brand/Delete/5
        [Authorize]
        [HttpPost, ActionName("ReActivate")]
        public ActionResult ReActivateConfirmed(int id)
        {
            var userToUpdate = db.Coupon_Code.SingleOrDefault(u => u.Cupn_Id == id);
            if (userToUpdate != null)
            {

                userToUpdate.Active = true;
                userToUpdate.ModifiedUser = WebSecurity.CurrentUserId;
                userToUpdate.ModifiedDate = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Coupon_Message"] = ConfigurationManager.AppSettings["RAV_SUC"];
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}