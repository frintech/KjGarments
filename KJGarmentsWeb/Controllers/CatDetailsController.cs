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
    public class CatDetailsController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /CatDetails/
        
        public ActionResult Index(int brandid, int sub_cat_id, int cat_id = 0, int page = 1)
        {
            IEnumerable<MnuSubcatProductDetails> catlist = null;
            if (cat_id == 0)
            {


                catlist = from p in db.PRODUCTs
                          join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                          join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                          where p.Brand_id == brandid && p.IsActive == true && I.First_Flag == true && p.Sub_Category_id == sub_cat_id
                          orderby p.Pid descending
                          select new MnuSubcatProductDetails
                          {
                              Pid = p.Pid,
                              ProductCode = p.PDT_CODE,
                              ProductName = p.PDT_Name,
                              ProductDesc = p.PDT_Description,
                              Imagepath = I.Image_Path,
                              Price = p.PDT_Price,
                              Product_Offer = p.PDT_offer,
                              offer = o.Ofr_Value_type,
                              brandid=p.Brand_id,
                              subcatid=p.Sub_Category_id,                          
                              DiscountPrice = p.offer_value_type_id == 1 ? (p.PDT_Price - (p.PDT_offer * p.PDT_Price) / 100) : p.PDT_Price - p.PDT_offer,
                          };
                if (catlist != null)
                {
                    ViewBag.brndid = catlist.FirstOrDefault()==null? 0 : catlist.FirstOrDefault().brandid;
                    ViewBag.subcatid = catlist.FirstOrDefault() == null ? 0 : catlist.FirstOrDefault().subcatid;
                }
            }
            else
            {
                catlist = from p in db.PRODUCTs
                          join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                          join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                          where p.IsActive == true && I.First_Flag == true && p.Category_id == cat_id
                          orderby p.Pid descending
                          select new MnuSubcatProductDetails
                          {
                              Pid = p.Pid,
                              ProductCode = p.PDT_CODE,
                              ProductName = p.PDT_Name,
                              ProductDesc = p.PDT_Description,
                              Imagepath = I.Image_Path,
                              Price = p.PDT_Price,
                              Product_Offer = p.PDT_offer,
                              offer = o.Ofr_Value_type,
                              brandid = p.Brand_id,
                              subcatid = p.Sub_Category_id,
                              DiscountPrice = p.offer_value_type_id == 1 ? (p.PDT_Price - (p.PDT_offer * p.PDT_Price) / 100) : p.PDT_Price - p.PDT_offer,
                          };
            }
           
            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["HomPageimgCnt"]);
            var products = (from p in catlist select p).ToList().OrderByDescending(x => x.Pid);
            return View(products.ToPagedList(page, pagesize));
           // return View(catlist.ToList());
           
            
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}