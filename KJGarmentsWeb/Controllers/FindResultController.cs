using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;
using KJGarmentsWeb.Models;
using System.Configuration;
using PagedList;

namespace KJGarmentsWeb.Controllers
{
    public class FindResultController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /FindResult/

        public ActionResult Index(string query, int page = 1)
        {
            int HomPageimgCnt = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["HomPageimgCnt"]);
            IEnumerable<FindresultModel> catlist = null;
            catlist = from p in db.PRODUCTs
                        join m in db.Menus on p.Category_id equals m.Mnu_id
                        join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                        join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                        from sbtype in db.Menus.Where(e => e.Mnu_id == p.Sub_Category_id).DefaultIfEmpty()
                        from brnd in db.Brands.Where(b => b.BRND_id == p.Brand_id).DefaultIfEmpty()
                      where im.First_Flag == true
                        select new FindresultModel
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
                            Cat_name = m.Mnu_Name == null ? "" : m.Mnu_Name,
                            Sub_cat_name = sbtype.Mnu_Name == null ? "" : sbtype.Mnu_Name,
                            brand_name = brnd.BRND_Name == null ? "" : brnd.BRND_Name
                        };

            //FindresultModel prddetailsview = null;
            //foreach (var m in model)
            //{
            //    prddetailsview = new FindresultModel();
            //    prddetailsview.Pid = m.Pid;
            //    prddetailsview.Product_Code = m.Product_Code;
            //    prddetailsview.Product_Name = m.Product_Name;
            //    prddetailsview.Product_Description = m.Product_Description;
            //    prddetailsview.Product_Price = m.Product_Price;
            //    prddetailsview.Category_id = m.Category_id;
            //    prddetailsview.Sub_Category_id = m.Sub_Category_id;
            //    prddetailsview.offer_id = m.offer_id;
            //    prddetailsview.Cat_name = m.Imagepath;
            //    prddetailsview.offer_id = m.offer_id;
            //    prddetailsview.Imagepath = m.Imagepath;
            //    prddetailsview.Cat_name=m.Cat_name;
            //    prddetailsview.Sub_cat_name = m.Sub_cat_name;
            //    prddetailsview.brand_name = m.brand_name;

            //}

            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["HomPageimgCnt"]);
            IEnumerable<FindresultModel> products = (from p in catlist select p).ToList();
            IEnumerable<FindresultModel> results = null;
            ViewBag.SearhTerm = query;
            if (products.Any(b => b.brand_name.ToUpper().Contains(query.ToUpper())))
            {
                results = products.Where(b => b.brand_name.ToUpper().Contains(query.ToUpper())).ToList();
            }

            if (products.Any(b=>b.Product_Description.ToUpper().Contains(query.ToUpper())))
            {

                results = products.Where(b => b.Product_Description.ToUpper().Contains(query.ToUpper())).ToList();
               
            }
            if (products.Any(b => b.Product_Name.ToUpper().Contains(query.ToUpper())))
            {
                results = products.Where(b => b.Product_Name.ToUpper().Contains(query.ToUpper())).ToList();
            }

            if (products.Any(b => b.Product_Price.ToString().ToUpper().Contains(query.ToUpper())))
            {

                results = products.Where(b => b.Product_Price.ToString().ToUpper().Contains(query.ToUpper())).ToList();

            }
            if (products.Any(b => b.Sub_cat_name.ToUpper().Contains(query.ToUpper())))
            {
                results = products.Where(b => b.Sub_cat_name.ToUpper().Contains(query.ToUpper())).ToList();
            }

            if (products.Any(b => b.Cat_name.ToUpper().Contains(query.ToUpper())))
            {

                results = products.Where(b => b.Cat_name.ToUpper().Contains(query.ToUpper())).ToList();

            }
            if (products.Any(b => b.Product_Code.ToUpper().Contains(query.ToUpper())))
            {

                results = products.Where(b => b.Product_Code.ToUpper().Contains(query.ToUpper())).ToList();

            }




            if (results != null)
            {


                return View(results.ToPagedList(page, pagesize));
            }
            else
            {
                TempData["Emptymsg"] = "Test";
                results = products;
                return View(results.ToPagedList(page, pagesize));
            }
        }

        //public static bool ContainsAny<T>(this IEnumerable<T> Collection, IEnumerable<T> Values)
        //{
        //    return Collection.Any(x => Values.Contains(x));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}