using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using System.Data.Objects;

namespace KJGarmentsWeb.Controllers
{
    public class ImageDetailsController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /ImageDetails/

        public ActionResult Index(int id)
        {
            //id = 8;
            var model = from p in db.PRODUCTs
                        join m in db.Menus on p.Category_id equals m.Mnu_id                     
                        join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                        join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                        from sbtype in db.Menus.Where(e => e.Mnu_id == p.Sub_Category_id).DefaultIfEmpty()
                        from brnd in db.Brands.Where(b => b.BRND_id == p.Brand_id).DefaultIfEmpty()  
                        where p.Pid == id && im.First_Flag == true
                        select new ProductdetailsView
                        {
                            Pid = p.Pid,
                            Product_Code = p.PDT_CODE,
                            Product_Name = p.PDT_Name,
                            Product_Description = p.PDT_Description,
                            Product_Price = p.PDT_Price,
                            Category_id = p.Category_id,
                            Sub_Category_id = p.Sub_Category_id,
                            offer_id = p.offer_value_type_id,
                            brand_id = p.Brand_id,
                            Imagepath = im.Image_Path,
                            Bannerflag = im.Banner_Flag,
                            Firstflag = im.First_Flag,
                            Homeflag = im.Home_Flag,
                            Product_offer = p.PDT_offer,
                            discount_price=p.PDT_Discount_price,
                            offer_type=o.Ofr_Value_type
                        };

            ProductdetailsView prddetailsview = null;
            foreach (var m in model)
            {
                prddetailsview = new ProductdetailsView();
                prddetailsview.Pid = m.Pid;
                prddetailsview.Product_Code = m.Product_Code;
                prddetailsview.Product_Name = m.Product_Name;
                prddetailsview.Product_Description = m.Product_Description;
                prddetailsview.Product_Price = m.Product_Price;
                prddetailsview.Category_id = m.Category_id;
                prddetailsview.Sub_Category_id = m.Sub_Category_id;
                prddetailsview.offer_id = m.offer_id;
                prddetailsview.Imagepath = m.Imagepath;
                prddetailsview.brand_id = m.brand_id;
                prddetailsview.Bannerflag = m.Bannerflag;
                prddetailsview.Firstflag = m.Firstflag;
                prddetailsview.Homeflag = m.Homeflag;
                //prddetailsview.Product_offer = m.Product_offer;
               // prddetailsview.discount_price = m.discount_price;
                prddetailsview.offer_type = m.offer_type;
                
            }
            if (model != null)
            {
                var sideimages = (from i in db.PDT_IMAGE_PATH
                                  where i.PDT_ID == id && i.First_Flag == false && i.Banner_Flag == false
                                  select i.Image_Path).ToList();

                prddetailsview.ImageList = sideimages;
                var offer = db.Sp_GetOffer();
                var productoffer = from o in db.ProductOfferValues
                                   where o.Pid == id
                                   select o;
                foreach (var ofritem in productoffer)
                {
                    prddetailsview.Product_offer = Convert.ToDecimal(ofritem.offer);
                    prddetailsview.discount_price = ofritem.OfrValtype == 1 ? (prddetailsview.Product_Price - (ofritem.offer * prddetailsview.Product_Price) / 100) : prddetailsview.Product_Price - ofritem.offer;
                }
            }
           
           

            foreach (var p in prddetailsview.GetType().GetProperties())
            {
                

                var subscription = from v in db.Value_Specification
                                   join K in db.KEY_Specifications on v.Key_spec_id equals K.spec_col_id
                                   where v.Product_id == id
                                   select new { K.spec_col_Name, v.key_Col_Value };

                if (subscription.Any(u => u.spec_col_Name == p.Name))
                {
                    foreach (var s in subscription)
                    {
                        if (s.spec_col_Name == p.Name)
                        {
                            prddetailsview.GetType().GetProperty(p.Name).SetValue(prddetailsview, s.key_Col_Value, null);
                        }
                    }
                }




            }
           
            return View(prddetailsview);

           
        }
        public JsonResult CheckValidpromoCode(string pdt_code)
        {
            var chkexists=from c in db.Coupon_Code
                           where c.Active == true && c.Cupn_Code == pdt_code
                           //&& EntityFunctions.TruncateTime(c.StartDate) >= System.DateTime.Now.Date && EntityFunctions.TruncateTime(c.EndDate) <= System.DateTime.Now.Date
                          select c;
            return Json(chkexists.ToList());
        }

        
    }
}