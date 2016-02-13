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
using WebMatrix.WebData;
using PagedList;
namespace KJGarmentsWeb.Controllers
{
    public class BannerImageController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /BannerImage/

        public ActionResult Index(int page = 1)
        {
            IEnumerable<ViewBannerImageModel> model = null;
            model = from p in db.Banner_Image_Dtl
                    select new ViewBannerImageModel
                    {
                        Imagepath = p.BNR_Image_Path,
                        BNRImgId = p.BNR_Img_Id,
                        Active = p.IsActive,

                    };
            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList().OrderByDescending(x=>x.BNRImgId);
            return View(products.ToPagedList(page, pagesize));
        }

        //
        // GET: /BannerImage/Details/5

        public ActionResult Details(long id = 0)
        {
            Banner_Image_Dtl banner_image_dtl = db.Banner_Image_Dtl.Find(id);
            if (banner_image_dtl == null)
            {
                return HttpNotFound();
            }
            return View(banner_image_dtl);
        }

        //
        // GET: /BannerImage/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BannerImage/Create

        [HttpPost]
        public ActionResult Create(AddBannerImageModel bannerimg)
        {
            if (ModelState.IsValid)
            {
                string ImageName = System.IO.Path.GetFileName(bannerimg.Imagepath.FileName);
                string physicalPath = Server.MapPath(ConfigurationManager.AppSettings["BNR_IMG_Path"] + ImageName);
                bannerimg.Imagepath.SaveAs(physicalPath);               
                Banner_Image_Dtl objImages = new Banner_Image_Dtl();
                objImages.BNR_Img_Id = (db.Banner_Image_Dtl.Max(i => (int?)i.BNR_Img_Id) ?? 0) + 1;
                objImages.BNR_Image_Path = ImageName;
                objImages.IsActive = true;               
                objImages.Add_user = WebSecurity.CurrentUserId;
                objImages.Add_DateTime = System.DateTime.Now;
                objImages.Mod_User = WebSecurity.CurrentUserId;
                objImages.Mod_DateTime = System.DateTime.Now;
                db.Banner_Image_Dtl.Add(objImages);
                db.SaveChanges();
                TempData["Message"] = ConfigurationManager.AppSettings["INS_SUC"];
                ModelState.Clear();
                return RedirectToAction("Create", "BannerImage");
            }

            return View(bannerimg);
        }



        //
        // GET: /Admin/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {

            var model = (from p in db.Banner_Image_Dtl
                         where p.BNR_Img_Id == id
                         select new deleteBannerImageModel
                         {
                             BNRImgId = p.BNR_Img_Id,
                             Imagepath = p.BNR_Image_Path,

                         }).ToList();
            deleteBannerImageModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteBannerImageModel();
                delbrnadmodel.BNRImgId = m.BNRImgId;
                delbrnadmodel.Imagepath = m.Imagepath;
            

            }

            return View(delbrnadmodel);

        }

        //
        // POST: /Admin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var userToUpdate = db.Banner_Image_Dtl.SingleOrDefault(u => u.BNR_Img_Id == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = false;
                userToUpdate.Mod_User = WebSecurity.CurrentUserId;
                userToUpdate.Mod_DateTime = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Message"] = ConfigurationManager.AppSettings["DEL_SUC"];
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = (from p in db.Banner_Image_Dtl
                         where p.BNR_Img_Id == id
                         select new deleteBannerImageModel
                         {
                             BNRImgId = p.BNR_Img_Id,
                             Imagepath = p.BNR_Image_Path,

                         }).ToList();
            deleteBannerImageModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteBannerImageModel();
                delbrnadmodel.BNRImgId = m.BNRImgId;
                delbrnadmodel.Imagepath = m.Imagepath;


            }
        
            return View(delbrnadmodel);

        }
        [HttpPost, ActionName("ReActivate")]
        [ValidateAntiForgeryToken]
        public ActionResult ReActivateConfirmed(int id)
        {

            var userToUpdate = db.Banner_Image_Dtl.SingleOrDefault(u => u.BNR_Img_Id == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = true;
                userToUpdate.Mod_User = WebSecurity.CurrentUserId;
                userToUpdate.Mod_DateTime = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["Message"] = ConfigurationManager.AppSettings["RAV_SUC"];
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}