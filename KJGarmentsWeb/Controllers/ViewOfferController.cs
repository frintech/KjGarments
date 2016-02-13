using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using PagedList;
using System.Configuration;
namespace KJGarmentsWeb.Controllers
{
    public class ViewOfferController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /ViewOffer/

        public ActionResult Index(int page = 1)
        {

           

            //int offerid = 5;
            //var alldiscount = (from S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == offerid)                              
            //                  where S.PDT_TYPE == "A"
            //                  select new {Discnt = S.DiscountPct });

            //var allproduct = (from S in db.PRODUCTs.Where(x => x.IsActive == true)                             
            //                   select new{Pid= S.Pid});
            //List<ViewofferDetails> Alloffer = new List<ViewofferDetails>();
            //foreach (var d in alldiscount)
            //{
            //    foreach (var I in allproduct)
            //    {

            //        ViewofferDetails obj = new ViewofferDetails();
            //        obj.Pid = I.Pid;
            //        obj.Offer = Convert.ToInt32(alldiscount.First().Discnt);
            //        Alloffer.Add(obj);

            //    }
            //}

            //var product = (from S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == offerid)
            //                     join P in db.PRODUCTs on S.ProductID equals P.Pid
            //               where S.PDT_TYPE == "P" && P.IsActive == true
            //                     select new { Id = P.Pid, Discnt = S.DiscountPct });

            //var menu = (from S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == offerid)
            //            join P in db.PRODUCTs on S.ProductID equals P.Category_id
            //            where S.PDT_TYPE == "T" && P.IsActive == true
            //            select new { Id = P.Pid, Discnt = S.DiscountPct }
            //              );
            //         var Subcate=(
            //                   from S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == offerid)
            //                   join P in db.PRODUCTs on S.ProductID equals P.Sub_Category_id
            //                   where S.PDT_TYPE == "T" && P.IsActive==true
            //                   select new { Id = P.Pid, Discnt = S.DiscountPct }
            //                   );
            //       var Brand=(
            //                   from S in db.SpecialOfferProducts.Where(x => x.SpecialOfferID == offerid)
            //                   join P in db.PRODUCTs on S.ProductID equals P.Brand_id
            //                   where S.PDT_TYPE == "B" && P.IsActive == true
            //                   select new { Id = P.Pid, Discnt = S.DiscountPct }
            //                   );

                 
            //       List<ViewofferDetails> Offerview = new List<ViewofferDetails>();
            //       foreach (var I in product)
            //       {
            //           if (!Offerview.Any(p => p.Pid == I.Id))
            //           {
            //               ViewofferDetails obj = new ViewofferDetails();
            //               obj.Pid = I.Id;
            //               obj.Offer = Convert.ToInt32(I.Discnt);
            //               Offerview.Add(obj);
            //           }
            //       }
            //       foreach (var I in Brand)
            //       {
            //           if (!Offerview.Any(p => p.Pid == I.Id))
            //           {
            //               ViewofferDetails obj = new ViewofferDetails();
            //               obj.Pid = I.Id;
            //               obj.Offer = Convert.ToInt32(I.Discnt);
            //               Offerview.Add(obj);
            //           }
            //       }
            //       foreach (var I in Subcate)
            //       {
            //           if (!Offerview.Any(p => p.Pid == I.Id))
            //           {
            //               ViewofferDetails obj = new ViewofferDetails();
            //               obj.Pid = I.Id;
            //               obj.Offer = Convert.ToInt32(I.Discnt);
            //               Offerview.Add(obj);
            //           }
            //       }
            //       foreach (var I in menu)
            //       {
            //           if (!Offerview.Any(p => p.Pid == I.Id))
            //           {
            //               ViewofferDetails obj = new ViewofferDetails();
            //               obj.Pid = I.Id;
            //               obj.Offer = Convert.ToInt32(I.Discnt);
            //               Offerview.Add(obj);
            //           }
            //       }
            //       foreach (var I in Alloffer)
            //       {
            //           if (!Offerview.Any(p => p.Pid == I.Pid))
            //           {
            //               ViewofferDetails obj = new ViewofferDetails();
            //               obj.Pid = I.Pid;
            //               obj.Offer = I.Offer;
            //               Offerview.Add(obj);
            //           }
            //       }

            var offer = db.Sp_GetOffer();
            
            int HomPageimgCnt = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HomPageimgCnt"]);
            IEnumerable<MnuSubcatProductDetails> catlist = null;
            catlist = (from p in db.PRODUCTs
                       join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                       join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                       join L in db.ProductOfferValues on p.Pid equals L.Pid                       
                       where p.IsActive == true && I.First_Flag == true 
                       && (from im in db.PDT_IMAGE_PATH
                           where im.Home_Flag == true
                           select im.PDT_ID).Contains(p.Pid)
                       orderby p.Pid descending
                       select new MnuSubcatProductDetails
                       {
                           Pid = p.Pid,
                           ProductCode = p.PDT_CODE,
                           ProductName = p.PDT_Name,
                           ProductDesc = p.PDT_Description,
                           Imagepath = I.Image_Path,
                           Price = p.PDT_Price,
                           Product_Offer =L.offer,// OV.Offer, //p.PDT_offer,
                           offer = o.Ofr_Value_type,
                           DiscountPrice = L.OfrValtype == 1 ? (p.PDT_Price - (L.offer * p.PDT_Price) / 100) : p.PDT_Price - L.offer,
                       });//.Take(HomPageimgCnt);

            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["HomPageimgCnt"]);
            var products = (from p in catlist select p).ToList(); //.OrderByDescending(x => x.Pid)
            return View(products.ToPagedList(page, pagesize));
           
        }

        //
        // GET: /ViewOffer/Details/5

        public ActionResult Details(int id = 0)
        {
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        //
        // GET: /ViewOffer/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ViewOffer/Create

        [HttpPost]
        public ActionResult Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Brands.Add(brand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        //
        // GET: /ViewOffer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        //
        // POST: /ViewOffer/Edit/5

        [HttpPost]
        public ActionResult Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        //
        // GET: /ViewOffer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Brand brand = db.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        //
        // POST: /ViewOffer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Brand brand = db.Brands.Find(id);
            db.Brands.Remove(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}