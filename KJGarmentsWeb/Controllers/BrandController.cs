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

namespace KJGarmentsWeb.Controllers
{
    public class BrandController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /Brand/
         [Authorize]
        public ActionResult Index(int page = 1)
        {
            IEnumerable<ViewBrandModel> model = null;
            model = (from p in db.Brands
                     join m in db.Menus on p.cat_id equals m.Mnu_id                    
                     from sbtype in db.Menus.Where(e => e.Mnu_id == p.sub_cat_id).DefaultIfEmpty()
                     select new ViewBrandModel
                     {
                       BRND_Name=p.BRND_Name,
                       BRND_Description=p.BRND_Description,
                       BRND_Img_Path=p.BRND_Img_Path,
                       Is_Add_img=p.Is_Add_img,
                       IsActive=p.IsActive,
                       brndId=p.BRND_id,
                       sub_category_name=sbtype.Mnu_Name,
                       category_name=m.Mnu_Name
                     }
            );
          
            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList().OrderByDescending(x => x.brndId);
            return View(products.ToPagedList(page, pagesize));
        }

        //
        // GET: /Brand/Details/5
       

        //
        // GET: /Brand/Create
         [Authorize]
        public ActionResult Create()
        {
            var query = from c in db.Menus
                        where !(from o in db.menuTrees
                                where o.Parent_Menuid != 0
                                select o.Menu_id)
                               .Contains(c.Mnu_id)
                        select new { c.Mnu_id, c.Mnu_Name };
            ViewBag.Menus = new SelectList(query, "mnu_Id", "Mnu_Name", "--Select Category--");           
            var subcat = (from c in db.Menus
                          where (from o in db.menuTrees
                                 where o.Parent_Menuid != 0 
                                 select o.Menu_id)
                                 .Contains(c.Mnu_id)
                          select new { c.Mnu_id, c.Mnu_Name }).ToList();
            ViewBag.subcategory = new SelectList(subcat, "mnu_Id", "Mnu_Name", "--Select Sub-Category--");

            return View();
        }

        //
        // POST: /Brand/Create
         [Authorize]
        [HttpPost]
        public ActionResult Create(AddBrandModel brand)
        {

          //  var errors = ModelState
          //.Where(x => x.Value.Errors.Count > 0)
          //.Select(x => new { x.Key, x.Value.Errors })
          //.ToArray();
            if (ModelState.IsValid)
            {

                try
                {
                    // Insert Product
                    Brand objnewbrand = new Brand();
                    objnewbrand.BRND_id = (db.Brands.Max(i => (int?)i.BRND_id) ?? 0) + 1;  //Convert.ToInt32(db.PRODUCTs.Max(x => x.Pid)) + 1;
                    objnewbrand.BRND_Name = brand.BRND_Name;
                    objnewbrand.BRND_Description = brand.BRND_Description;
                    objnewbrand.IsActive = true;
                    objnewbrand.cat_id = brand.Category_id;
                    objnewbrand.sub_cat_id = brand.Sub_Category_id;
                    objnewbrand.BRND_Img_Path = string.Empty;
                    if (brand.BRND_Img_Path != null)
                    {
                        string ImageName = System.IO.Path.GetFileName(brand.BRND_Img_Path.FileName);
                        string physicalPath = Server.MapPath(ConfigurationManager.AppSettings["Mnu_ImgPath"] + ImageName);
                        brand.BRND_Img_Path.SaveAs(physicalPath);
                        //string targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        //brand.BRND_Img_Path.SaveAs(targetPath);
                        objnewbrand.BRND_Img_Path = ImageName;
                        objnewbrand.Is_Add_img = true;
                       
                    }
                    objnewbrand.Add_User_id = WebSecurity.CurrentUserId;
                    objnewbrand.Mod_User_Id = WebSecurity.CurrentUserId;
                    objnewbrand.Add_Datetime = System.DateTime.Now;
                    objnewbrand.Modify_Datetime = System.DateTime.Now;                    
                    db.Brands.Add(objnewbrand);
                    db.SaveChanges();
                    TempData["brnd_Message"] = ConfigurationManager.AppSettings["INS_SUC"];
                    ModelState.Clear();

                }

                catch (Exception exception)
                {
                 
                }

                return RedirectToAction("Create", "Brand");

            }

           
          
            return View(brand);
        }

        //
        // GET: /Brand/Edit/5
         [Authorize]
        public ActionResult Edit(int id = 0)
        {
            var model = from p in db.Brands
                        join m in db.Menus on p.cat_id equals m.Mnu_id
                        from sbtype in db.Menus.Where(e => e.Mnu_id == p.sub_cat_id).DefaultIfEmpty()
                        where p.BRND_id == id 
                        select new EditBrandModel
                        {
                           brndId=p.BRND_id,
                           BRND_Name=p.BRND_Name,
                           BRND_Description=p.BRND_Description,
                           Category_id=p.cat_id,
                           Sub_Category_id=p.sub_cat_id,
                          BRND_Img_Path_name=p.BRND_Img_Path

                        };
            EditBrandModel editprddetails = null;
            foreach (var m in model)
            {
                editprddetails = new EditBrandModel();
                editprddetails.brndId = m.brndId;
                editprddetails.BRND_Name = m.BRND_Name;
                editprddetails.BRND_Description = m.BRND_Description;
                editprddetails.Category_id = m.Category_id;
                editprddetails.Sub_Category_id = m.Sub_Category_id;
                editprddetails.BRND_Img_Path_name = m.BRND_Img_Path_name;
            }

            var query = from c in db.Menus
                        where !(from o in db.menuTrees
                                where o.Parent_Menuid != 0
                                select o.Menu_id)
                               .Contains(c.Mnu_id)
                        select new { c.Mnu_id, c.Mnu_Name };
            ViewBag.Menus = new SelectList(query, "mnu_Id", "Mnu_Name", editprddetails.Category_id);         

          

            var subcat = (from c in db.Menus
                          where (from o in db.menuTrees
                                 where o.Parent_Menuid != 0 && o.Parent_Menuid == editprddetails.Category_id
                                 select o.Menu_id)
                                 .Contains(c.Mnu_id)
                          select new { c.Mnu_id, c.Mnu_Name }).ToList();
            ViewBag.subcategory = new SelectList(subcat, "mnu_Id", "Mnu_Name", editprddetails.Sub_Category_id);

            return View(editprddetails);
        }

        //
        // POST: /Brand/Edit/5
         [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBrandModel brand)
        {
            var errors = ModelState
          .Where(x => x.Value.Errors.Count > 0)
          .Select(x => new { x.Key, x.Value.Errors })
          .ToArray();
            if (ModelState.IsValid)
            {
                var userToUpdatebrands = db.Brands.SingleOrDefault(u => u.BRND_id == brand.brndId);
            if (userToUpdatebrands != null)
            {
                userToUpdatebrands.BRND_Name = brand.BRND_Name;
                
                userToUpdatebrands.BRND_Description = brand.BRND_Description;
                if (brand.BRND_Img_Path != null)
                {
                    string ImageName = System.IO.Path.GetFileName(brand.BRND_Img_Path.FileName);
                    string physicalPath = Server.MapPath(ConfigurationManager.AppSettings["Mnu_ImgPath"] + ImageName);
                    brand.BRND_Img_Path.SaveAs(physicalPath);
                    //string targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                    //brand.BRND_Img_Path.SaveAs(targetPath);
                    userToUpdatebrands.BRND_Img_Path = ImageName;
                    userToUpdatebrands.Is_Add_img = true;

                }     
                userToUpdatebrands.Mod_User_Id= WebSecurity.CurrentUserId;
                userToUpdatebrands.Modify_Datetime = System.DateTime.Now;
               
                db.SaveChanges();
            }
            TempData["brnd_Message"] = ConfigurationManager.AppSettings["EDT_SUC"];
                return RedirectToAction("Index");
              
            }
            return View(brand);
        }

        //
        // GET: /Brand/Delete/5
         [Authorize]
        public ActionResult Delete(int id = 0)
        {

            var model = from p in db.Brands
                        join m in db.Menus on p.cat_id equals m.Mnu_id
                        from sbtype in db.Menus.Where(e => e.Mnu_id == p.sub_cat_id).DefaultIfEmpty()
                        where p.BRND_id == id
                        select new deleteBrandModel
                        {
                            brndId = p.BRND_id,
                            BRND_Name = p.BRND_Name,
                            BRND_Description = p.BRND_Description,
                         

                        };
            deleteBrandModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteBrandModel();
                delbrnadmodel.brndId = m.brndId;
                delbrnadmodel.BRND_Name = m.BRND_Name;
                delbrnadmodel.BRND_Description = m.BRND_Description;
               
            }

            return View(delbrnadmodel);
        }

        //
        // POST: /Brand/Delete/5
         [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var userToUpdate = db.Brands.SingleOrDefault(u => u.BRND_id == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = false;
                userToUpdate.Mod_User_Id = WebSecurity.CurrentUserId;
                userToUpdate.Modify_Datetime = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["brnd_Message"] = ConfigurationManager.AppSettings["DEL_SUC"];
            return RedirectToAction("Index");
        }
         [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = from p in db.Brands
                        join m in db.Menus on p.cat_id equals m.Mnu_id
                        from sbtype in db.Menus.Where(e => e.Mnu_id == p.sub_cat_id).DefaultIfEmpty()
                        where p.BRND_id == id
                        select new deleteBrandModel
                        {
                            brndId = p.BRND_id,
                            BRND_Name = p.BRND_Name,
                            BRND_Description = p.BRND_Description,


                        };
            deleteBrandModel delbrnadmodel = null;
            foreach (var m in model)
            {
                delbrnadmodel = new deleteBrandModel();
                delbrnadmodel.brndId = m.brndId;
                delbrnadmodel.BRND_Name = m.BRND_Name;
                delbrnadmodel.BRND_Description = m.BRND_Description;

            }

            return View(delbrnadmodel);
        }

        //
        // POST: /Brand/Delete/5
         [Authorize]
        [HttpPost, ActionName("ReActivate")]
        public ActionResult ReActivateConfirmed(int id)
        {
            var userToUpdate = db.Brands.SingleOrDefault(u => u.BRND_id == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = true;
                userToUpdate.Mod_User_Id = WebSecurity.CurrentUserId;
                userToUpdate.Modify_Datetime = System.DateTime.Now;

                db.SaveChanges();
            }
            TempData["brnd_Message"] = ConfigurationManager.AppSettings["RAV_SUC"];
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}