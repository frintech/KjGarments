using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using KJGarmentsWeb.Filters;
using PagedList;
using System.Configuration;
using System.IO;

namespace KJGarmentsWeb.Controllers
{
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        private MughapuEntities db = new MughapuEntities();
        public ActionResult Index(int catid=0,string type="A",int page=1,int sortby=1)
        {
            if (Session["UsrCount"] == null)
            {
                string body = string.Empty;
                Session["UsrCount"] = "1";
             
                using (var sr = new StreamReader(Server.MapPath(ConfigurationManager.AppSettings["VisitorsCount"])))
                {
                    body = (Convert.ToInt64(sr.ReadToEnd()) +1).ToString();

                }
                using (var sr = new StreamWriter(Server.MapPath(ConfigurationManager.AppSettings["VisitorsCount"])))
                {
                    sr.Write(body);

                }
            }
          

            int HomPageimgCnt = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HomPageimgCnt"]);
            IEnumerable<MnuSubcatProductDetails> catlist = null;
            if (catid == 0)
            {
                catlist = (from p in db.PRODUCTs
                           join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                           join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
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
                               Product_Offer = p.PDT_offer,
                               offer = o.Ofr_Value_type,
                               DiscountPrice = p.offer_value_type_id == 1 ? (p.PDT_Price - (p.PDT_offer * p.PDT_Price) / 100) : p.PDT_Price - p.PDT_offer,
                           });
            }
            else
            {
                if (type == "A")
                {
                    catlist = (from p in db.PRODUCTs
                               join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                               join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                               where p.IsActive == true && I.First_Flag == true && p.Category_id == catid
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
                                   Product_Offer = p.PDT_offer,
                                   offer = o.Ofr_Value_type,
                                   DiscountPrice = p.offer_value_type_id == 1 ? (p.PDT_Price - (p.PDT_offer * p.PDT_Price) / 100) : p.PDT_Price - p.PDT_offer,
                               });
                }
                else
                {
                    catlist = (from p in db.PRODUCTs
                               join I in db.PDT_IMAGE_PATH on p.Pid equals I.PDT_ID
                               join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                               where p.IsActive == true && I.First_Flag == true && p.Sub_Category_id == catid
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
                                   Product_Offer = p.PDT_offer,
                                   offer = o.Ofr_Value_type,
                                   DiscountPrice = p.offer_value_type_id == 1 ? (p.PDT_Price - (p.PDT_offer * p.PDT_Price) / 100) : p.PDT_Price - p.PDT_offer,
                               });
                }
            }
           

            //var query = (from c in db.PDT_IMAGE_PATH
            //             where (from o in db.PRODUCTs
            //                    where o.IsActive == true
            //                    select o.Pid)
            //                    .Contains(c.PDT_ID) && c.Banner_Flag == true
            //             select new EditImageItem { ImgId = c.img_id, Imagepath = c.Image_Path, Imagetype = c.Img_type });
            var query = (from c in db.Banner_Image_Dtl.Where(x=>x.IsActive==true)
                         select new BannerImagedetails { ImgId = c.BNR_Img_Id, Imagepath = c.BNR_Image_Path });

           ViewBag.BannerImage = query.ToList();
           var list = new SelectList(new[] { new { ID = "1", Name = "Last added product" }, new { ID = "2", Name = "Price==>High to Low" }, 
               new { ID = "4", Name = "Z=>A by Name" }, new { ID = "5", Name = "A=>Z by Name" },new { ID = "3", Name = "Price==>Low to High" }
           }, "ID", "Name", 1); 
         
           ViewBag.SortbyCat = list;


           int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["HomPageimgCnt"]);
           var products = (from p in catlist select p).ToList().OrderByDescending(x => x.Pid);
             if (sortby == 1)
             {
                 products = products.OrderByDescending(x => x.Pid);
             }
             if (sortby == 6)
             {
                 products = products.OrderBy(x => x.Pid);
             }
             else if (sortby == 2)
             {
                 products = products.OrderByDescending(x => x.Price);
             }
             else if (sortby == 3)
             {
                 products = products.OrderBy(x => x.Price);
             }
             else if (sortby == 4)
             {
                 products = products.OrderBy(x => x.ProductName);
             }
             else if (sortby == 5)
             {
                 products = products.OrderByDescending(x => x.ProductName);
             }

            
           return View(products.ToPagedList(page, pagesize));
           // return View(catlist.ToList());

            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Search()
        {
            return PartialView("Search");
        }
        [HttpPost]

        public ActionResult Search(string query)
        {
           if(!string.IsNullOrEmpty(query))
           {

               return RedirectToAction("Index", "FindResult", new { query = query });
           }

            return PartialView();

        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       public PartialViewResult Menu()
    {
        List<MenuBrandView> objlst = new List<MenuBrandView>();
        var ChargeTypes = db.Brands.Where(x=>x.IsActive==true).ToList();
           foreach(var c in ChargeTypes)
           {
               MenuBrandView obj = new MenuBrandView();
               obj.MnuBrnandId = c.BRND_id;
               obj.Brandname = c.BRND_Name;
               obj.catid =Convert.ToInt32(c.cat_id);
               obj.Subcatid = Convert.ToInt32(c.sub_cat_id);
               obj.ImagePath = c.BRND_Img_Path;
               objlst.Add(obj);
           }
           return PartialView(objlst);
       }
        
       
    }
}
