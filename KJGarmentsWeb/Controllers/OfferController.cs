

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
    public class OfferController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /Offer/
        [Authorize]
        public ActionResult Index(int page = 1)
        {
            IEnumerable<ViewofferModel> model = null;
            model = (from p in db.SpecialOffers
                     select new ViewofferModel
                     {

                         offer_Code = p.offer_Code,
                         offer_Name = p.offer_Name,
                         Description = p.Description,
                         Type = p.Type,
                         usr_Category = p.usr_Category,
                         StartDate = p.StartDate,
                         EndDate = p.EndDate,
                         MinQty = p.MinQty,
                         MaxQty = p.MaxQty,
                         Isactive = p.Active,
                         SpecialOfferID = p.SpecialOfferID
                     }
            );

            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList().OrderByDescending(x => x.SpecialOfferID);
            return View(products.ToPagedList(page, pagesize));
        }
        private void SetChildren(OfferModel model, List<OfferModel> userList)
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
        private void EditSetChildren(EditOfferModel model, List<EditOfferModel> userList, int splofferid)
        {
            var childs = userList.
                            Where(x => x.parent_name == model.Item_Name).ToList();
            if (childs.Count > 0)
            {
                foreach (var child in childs)
                {
                    EditSetChildren(child, userList, splofferid);
                    model.tvlists.Add(child);
                }
            }
        }
        //
        // GET: /Offer/Details/5

        public ActionResult Details(int id = 0)
        {
            SpecialOffer specialoffer = db.SpecialOffers.Find(id);
            if (specialoffer == null)
            {
                return HttpNotFound();
            }
            return View(specialoffer);
        }

        //
        // GET: /Offer/Create
        [Authorize]
        public ActionResult Create()
        {
            var treeview = db.GetTreeView();
            List<OfferModel> userList;

            userList = (from t in db.TreeviewLists
                        select new OfferModel
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
            var list = new SelectList(new[]{ new { ID = "U", Name = "User" }, new { ID = "O", Name = "Offer" }},  "ID", "Name",1);
            ViewBag.Type= list;
            president.offer_Code = GetVoucherNumber();
           
            return View(president);
        }

        private  Random random = new Random();

        public string GetVoucherNumber()
        {
            int length = 7;
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());           
            if (db.SpecialOffers.Where(x => x.offer_Code == result).Any())
            {
                GetVoucherNumber();
            }
            return result;
        }
        //
        // POST: /Offer/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(OfferModel offer)
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
                    int SpecialOfferID = (db.SpecialOffers.Max(i => (int?)i.SpecialOfferID) ?? 0) + 1;
                    SpecialOffer objnewOffer = new SpecialOffer();
                    // objnewOffer.SpecialOfferID = SpecialOfferID;
                    objnewOffer.offer_Code = offer.offer_Code;
                    objnewOffer.offer_Name = offer.offer_Name;
                    objnewOffer.Description = offer.Description;
                    objnewOffer.Type ="D";
                    objnewOffer.usr_Category ="O";
                    objnewOffer.StartDate = offer.StartDate;
                    objnewOffer.EndDate = offer.EndDate;
                    objnewOffer.MinQty = Convert.ToInt32(offer.MinQty);
                    objnewOffer.MaxQty = Convert.ToInt32(offer.MaxQty);
                    if (offer.AddBannerImgpath != null)
                    {
                        string ImageName = System.IO.Path.GetFileName(offer.AddBannerImgpath.FileName);
                        string physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        offer.AddBannerImgpath.SaveAs(physicalPath);
                        string targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        offer.AddBannerImgpath.SaveAs(targetPath);
                        objnewOffer.Image_Path_Name = ImageName;
                    }
                   // objnewOffer.Image_Path_Name = "";
                    objnewOffer.Active = true;
                    objnewOffer.Adduser = WebSecurity.CurrentUserId;
                    objnewOffer.ModifiedUser = WebSecurity.CurrentUserId;
                    objnewOffer.AddDatetime = System.DateTime.Now;
                    objnewOffer.ModifiedDate = System.DateTime.Now;
                    db.SpecialOffers.Add(objnewOffer);
                    db.SaveChanges();

                    foreach (var m in strPdtNames.Split('{'))
                    {

                        if (m != "[")
                        {
                            if(!string.IsNullOrEmpty(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", "")))
                            {
                            SpecialOfferProduct objOfferPdt = new SpecialOfferProduct();
                            objOfferPdt.SeqNo = (db.SpecialOfferProducts.Max(i => (int?)i.SeqNo) ?? 0) + 1;
                            objOfferPdt.SpecialOfferID = SpecialOfferID;
                            objOfferPdt.DiscountPct = Convert.ToInt32(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", ""));
                            objOfferPdt.OFR_VAL_TYPE = Convert.ToInt32(m.Split(',')[2].Split(':')[1].ToString().Replace("\"", ""));
                            objOfferPdt.ProductID = Convert.ToInt32(m.Split(',')[3].Split(':')[1].ToString().Replace("\"", ""));
                            objOfferPdt.PDT_TYPE = m.Split(',')[4].Split(':')[1].ToString().Replace("\"", "").Replace("}", "").Replace("]", "");
                            objOfferPdt.Adduser = WebSecurity.CurrentUserId;
                            objOfferPdt.ModifiedUser = WebSecurity.CurrentUserId;
                            objOfferPdt.AddDatetime = System.DateTime.Now;
                            objOfferPdt.ModifiedDate = System.DateTime.Now;
                            db.SpecialOfferProducts.Add(objOfferPdt);
                            db.SaveChanges();
                            }
                        }
                    }
                    TempData["Offer_Message"] = ConfigurationManager.AppSettings["INS_SUC"];


                    ModelState.Clear();

                    return RedirectToAction("Create", "Offer");
                }
                catch
                {
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    return View();
                }
            }
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            return RedirectToAction("Create", "Offer");
        }

        //
        // GET: /Offer/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {

            EditOfferModel editofferdetails = new EditOfferModel();
            var treeview = db.GetTreeView();
            List<EditOfferModel> userList=new List<EditOfferModel>();

            //userList = (from t in db.TreeviewLists
            //            join S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == id) on new { a = t.menu_id, b = t.origin } equals new { a = S.ProductID, b = S.PDT_TYPE }
            //            into tvGroup
            //            from p in tvGroup.DefaultIfEmpty()
            //            //where itemOffer.SpecialOfferID == id
            //            select new EditOfferModel
            //            {
            //                id = t.id,
            //                menu_id = t.menu_id,
            //                Parent_Menuid = t.Parent_Menuid,
            //                origin = t.origin,
            //                Item_Name = t.Item_Name,
            //                parent_name = t.parent_name,
            //                Discountpct = (p.DiscountPct == null ? null : p.DiscountPct),
            //                offer_id = (p.OFR_VAL_TYPE == null ? null : p.OFR_VAL_TYPE)

            //            }).ToList();

            editofferdetails = userList.
                              Where(x => x.parent_name == "XYA").FirstOrDefault();
            EditSetChildren(editofferdetails, userList, id);
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");

            var model = from p in db.SpecialOffers
                        where p.SpecialOfferID == id
                        select new
                        {
                            p.offer_Code,
                            p.offer_Name,
                            p.SpecialOfferID,
                            p.Type,
                            p.usr_Category,
                            p.Image_Path_Name,
                            p.MinQty,
                            p.MaxQty,
                            p.StartDate,
                            p.EndDate,
                            p.Description
                        };


            foreach (var m in model)
            {
                // editofferdetails = new EditOfferModel();
                editofferdetails.offer_Code = m.offer_Code;
                editofferdetails.offer_Name = m.offer_Name;
                editofferdetails.splofferid = m.SpecialOfferID;
                editofferdetails.Type = m.Type;
                editofferdetails.usr_Category = m.usr_Category;
                editofferdetails.AddBnrimageName = m.Image_Path_Name;
                editofferdetails.MinQty = m.MinQty;
                editofferdetails.MaxQty = m.MaxQty;
                editofferdetails.StartDate = m.StartDate;
                editofferdetails.EndDate = m.EndDate;
                editofferdetails.Description = m.Description;
            }






            return View(editofferdetails);
        }

        //
        // POST: /Offer/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(EditOfferModel edtoffer)
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

                    var userToUpdateproducts = db.SpecialOffers.SingleOrDefault(u => u.SpecialOfferID == edtoffer.splofferid);
                    if (userToUpdateproducts != null)
                    {
                       // userToUpdateproducts.offer_Code = edtoffer.offer_Code;
                        userToUpdateproducts.offer_Name = edtoffer.offer_Name;
                        userToUpdateproducts.Description = edtoffer.Description;
                        userToUpdateproducts.MinQty = Convert.ToInt32(edtoffer.MinQty);
                        userToUpdateproducts.MaxQty = Convert.ToInt32(edtoffer.MaxQty);
                        userToUpdateproducts.StartDate = edtoffer.StartDate;
                        userToUpdateproducts.EndDate = edtoffer.EndDate;
                        userToUpdateproducts.ModifiedUser = WebSecurity.CurrentUserId;
                        userToUpdateproducts.ModifiedDate = System.DateTime.Now;
                        //userToUpdateproducts.Type = edtoffer.Type;
                        if (edtoffer.AddBannerImgpath != null)
                        {
                                ImageName = System.IO.Path.GetFileName(edtoffer.AddBannerImgpath.FileName);
                                physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                                edtoffer.AddBannerImgpath.SaveAs(physicalPath);
                                targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                                edtoffer.AddBannerImgpath.SaveAs(targetPath);
                               userToUpdateproducts.Image_Path_Name=ImageName;
                           
                        }
                       // userToUpdateproducts.usr_Category = edtoffer.usr_Category;
                        db.SaveChanges();
                    }
                    IList<SpecialOfferProduct> tvItem = (from p in db.SpecialOfferProducts
                                               where p.SpecialOfferID == edtoffer.splofferid
                                      select p).ToList();
                    foreach (SpecialOfferProduct s in tvItem)
                    {
                    
                        db.SpecialOfferProducts.Remove(s);
                       
                    }
                    db.SaveChanges();
                

                    foreach (var m in strPdtNames.Split('{'))
                    {

                        if (m != "[")
                        {
                            if (!string.IsNullOrEmpty(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", "")))
                            {
                                SpecialOfferProduct objOfferPdt = new SpecialOfferProduct();
                                objOfferPdt.SeqNo = (db.SpecialOfferProducts.Max(i => (int?)i.SeqNo) ?? 0) + 1;
                                objOfferPdt.SpecialOfferID = edtoffer.splofferid;
                                objOfferPdt.DiscountPct = Convert.ToInt32(m.Split(',')[1].Split(':')[1].ToString().Replace("\"", ""));
                                objOfferPdt.OFR_VAL_TYPE = Convert.ToInt32(m.Split(',')[2].Split(':')[1].ToString().Replace("\"", ""));
                                objOfferPdt.ProductID = Convert.ToInt32(m.Split(',')[3].Split(':')[1].ToString().Replace("\"", ""));
                                objOfferPdt.PDT_TYPE = m.Split(',')[4].Split(':')[1].ToString().Replace("\"", "").Replace("}", "").Replace("]", "");
                                objOfferPdt.Adduser = WebSecurity.CurrentUserId;
                                objOfferPdt.ModifiedUser = WebSecurity.CurrentUserId;
                                objOfferPdt.AddDatetime = System.DateTime.Now;
                                objOfferPdt.ModifiedDate = System.DateTime.Now;
                                db.SpecialOfferProducts.Add(objOfferPdt);
                                db.SaveChanges();
                            }
                        }
                    }
                    TempData["Offer_Message"] = ConfigurationManager.AppSettings["EDT_SUC"];
                    return RedirectToAction("Index");

                    

                 
                }
                catch
                {
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    return View();
                }
            }
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            return View();
        }

        //
        // GET: /Offer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var model = from p in db.SpecialOffers
                        where p.SpecialOfferID == id
                        select new deleteOfferModel
                        {
                            SpecialOfferID = p.SpecialOfferID,
                            offer_Code = p.offer_Code,
                            offer_Name = p.offer_Name,


                        };

            deleteOfferModel deloffermodel = null;
            foreach (var m in model)
            {
                deloffermodel = new deleteOfferModel();
                deloffermodel.SpecialOfferID = m.SpecialOfferID;
                deloffermodel.offer_Code = m.offer_Code;
                deloffermodel.offer_Name = m.offer_Name;

            }

            return View(deloffermodel);

        }

        //
        // POST: /Offer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var userToUpdate = db.SpecialOffers.SingleOrDefault(u => u.SpecialOfferID == id);
            if (userToUpdate != null)
            {

                userToUpdate.Active = false;
                userToUpdate.ModifiedUser = WebSecurity.CurrentUserId;
                userToUpdate.ModifiedDate = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Offer_Message"] = ConfigurationManager.AppSettings["DEL_SUC"];
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = from p in db.SpecialOffers
                        where p.SpecialOfferID == id
                        select new deleteOfferModel
                        {
                            SpecialOfferID = p.SpecialOfferID,
                            offer_Code = p.offer_Code,
                            offer_Name = p.offer_Name,


                        };

            deleteOfferModel deloffermodel = null;
            foreach (var m in model)
            {
                deloffermodel = new deleteOfferModel();
                deloffermodel.SpecialOfferID = m.SpecialOfferID;
                deloffermodel.offer_Code = m.offer_Code;
                deloffermodel.offer_Name = m.offer_Name;

            }

            return View(deloffermodel);
        }

        //
        // POST: /Brand/Delete/5
        [Authorize]
        [HttpPost, ActionName("ReActivate")]
        public ActionResult ReActivateConfirmed(int id)
        {
            var userToUpdate = db.SpecialOffers.SingleOrDefault(u => u.SpecialOfferID == id);
            if (userToUpdate != null)
            {

                userToUpdate.Active = true;
                userToUpdate.ModifiedUser = WebSecurity.CurrentUserId;
                userToUpdate.ModifiedDate = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Offer_Message"] = ConfigurationManager.AppSettings["RAV_SUC"];
            return RedirectToAction("Index");

        }
        [Authorize]
        public JsonResult ChkOfferValid(string fromdate, string todate)
        {

            var frmdt = Convert.ToDateTime(fromdate);
            var todt = Convert.ToDateTime(todate); 

            var chkofferexists = from a in db.SpecialOffers
                                 where a.Active == true && ((frmdt <= a.StartDate && a.StartDate <= todt) && (frmdt <= a.EndDate && a.EndDate <= todt))
                                 select a;
            foreach (var d in chkofferexists)
            {
                return Json(chkofferexists);
            }
            return Json("false");
           

           
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}