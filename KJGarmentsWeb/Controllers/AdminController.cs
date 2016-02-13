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
    public class AdminController : Controller
    {
        private MughapuEntities db = new MughapuEntities();

        //
        // GET: /Admin/
         [Authorize]
        public ActionResult Index(int page = 1, string sortOrder="")
        {
            ViewBag.SortPdtName = String.IsNullOrEmpty(sortOrder) ? "Product_Name" : "";
            ViewBag.SortPdtCode = String.IsNullOrEmpty(sortOrder) ? "Product_Code" : "";

            ViewBag.SortPdtprice = String.IsNullOrEmpty(sortOrder) ? "Product_price" : "";
            ViewBag.SortPdtCat = String.IsNullOrEmpty(sortOrder) ? "Product_Cat" : "";
            ViewBag.SortPdtSubCat = String.IsNullOrEmpty(sortOrder) ? "Product_SubCat" : "";
            ViewBag.SortPdtbrand = String.IsNullOrEmpty(sortOrder) ? "Product_brand" : "";
            ViewBag.SortPdtofr = String.IsNullOrEmpty(sortOrder) ? "Product_offer" : "";

            IEnumerable<ViewProductdetails> model = null;
            model = (from p in db.PRODUCTs
                     join m in db.Menus on p.Category_id equals m.Mnu_id                    
                     join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                     join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                     from sbtype in db.Menus.Where(e => e.Mnu_id == p.Sub_Category_id).DefaultIfEmpty()
                     from brnd in db.Brands.Where(b => b.BRND_id == p.Brand_id).DefaultIfEmpty()  
                     where im.First_Flag == true
                     select new ViewProductdetails
                     {
                         Pid = p.Pid,
                         Product_Code = p.PDT_CODE,
                         Product_Name = p.PDT_Name,
                         Product_Description = p.PDT_Description,
                         Category = m.Mnu_Name,
                         Sub_Category = sbtype.Mnu_Name,
                         brand = brnd.BRND_Name,
                         offer = o.Ofr_Value_type,
                         Product_Price = p.PDT_Price,
                         ImagePath = im.Image_Path,
                         Product_offer = p.PDT_offer,
                         Isactive = p.IsActive

                     }
            );
            switch (sortOrder)
            {
                case "Product_Code":
                    model = model.OrderBy(s => s.Product_Code);
                    ViewBag.SortPdtCode = "Product_Code_desc";
                    break;
                case "Product_Code_desc":
                    model = model.OrderByDescending(s => s.Product_Code);
                    ViewBag.SortPdtCode = "Product_Code";
                    break;
                case "Product_Name_desc":
                    model = model.OrderByDescending(s => s.Product_Name);
                    ViewBag.SortPdtName = "Product_Name";
                    break;
                case "Product_Name":
                    model = model.OrderBy(s => s.Product_Name);
                    ViewBag.SortPdtName = "Product_Name_desc";
                    break;
                case "Product_price_desc":
                    model = model.OrderByDescending(s => s.Product_Price);
                    ViewBag.SortPdtprice = "Product_price";
                    break;
                case "Product_price":
                    model = model.OrderBy(s => s.Product_Price);
                    ViewBag.SortPdtprice = "Product_price_desc";
                    break;
                case "Product_Cat_desc":
                    model = model.OrderByDescending(s => s.Category);
                    ViewBag.SortPdtCat = "Product_Cat";
                    break;
                case "Product_Cat":
                    model = model.OrderBy(s => s.Category);
                    ViewBag.SortPdtCat = "Product_Cat_desc";
                    break;
                case "Product_SubCat_desc":
                    model = model.OrderByDescending(s => s.Sub_Category);
                    ViewBag.SortPdtSubCat = "Product_SubCat";
                    break;
                case "Product_SubCat":
                    model = model.OrderBy(s => s.Sub_Category);
                    ViewBag.SortPdtSubCat = "Product_SubCat_desc";
                    break;
                case "Product_brand_desc":
                    model = model.OrderByDescending(s => s.brand);
                    ViewBag.SortPdtbrand = "Product_brand";
                    break;
                case "Product_brand":
                    model = model.OrderBy(s => s.brand);
                    ViewBag.SortPdtbrand = "Product_brand_desc";
                    break;
                case "Product_offer":
                    model = model.OrderBy(s => s.offer);
                    ViewBag.SortPdtofr = "Product_offer_desc";
                    break;
                case "Product_offer_desc":
                    model = model.OrderByDescending(s => s.offer);
                    ViewBag.SortPdtofr = "Product_offer";
                    break;                    
                default:
                    model = model.OrderByDescending(s => s.Pid);
                    break;
            }
            var query = from c in db.Menus
                         where !(from o in db.menuTrees
                                 where o.Parent_Menuid != 0
                                 select o.Menu_id)
                                .Contains(c.Mnu_id)
                         select new { c.Mnu_id, c.Mnu_Name };
            ViewData["Menus"] = new SelectList(query, "mnu_Id", "Mnu_Name", "--Select Category--");
            ViewData["brands"] = new SelectList(db.Brands.Where(act => act.IsActive==true), "BRND_id", "BRND_Name", "--Select Brand--");
            int pagesize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            var products = (from p in model select p).ToList(); //.OrderByDescending(x => x.Pid)
            return View(products.ToPagedList(page, pagesize));
            
        }

        //
        // GET: /Admin/Details/5
         [Authorize]
        public ActionResult Details(int id = 0)
        {
            PRODUCT product = db.PRODUCTs.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // GET: /Admin/Create
         [Authorize]
        public ActionResult Create()
        {
            var query = from c in db.Menus
                        where !(from o in db.menuTrees
                                where o.Parent_Menuid != 0
                                select o.Menu_id)
                               .Contains(c.Mnu_id)
                        select new { c.Mnu_id, c.Mnu_Name };
            ViewBag.Menus  = new SelectList(query, "mnu_Id", "Mnu_Name", "--Select Category--");
            ViewBag.Brands = new SelectList(db.Brands.Where(act => act.IsActive==true), "BRND_id", "BRND_Name", "--Select Brand--");
            ViewBag.Sub_category = new SelectList(db.Spec_category, "spec_cat_id", "spec_cat_Name", "--Select Sub--");
            var subcat = (from c in db.Menus
                          where (from o in db.menuTrees
                                 where o.Parent_Menuid != 0
                                 select o.Menu_id)
                                 .Contains(c.Mnu_id)
                          select new { c.Mnu_id, c.Mnu_Name }).ToList();
            ViewBag.Sub_category = new SelectList(subcat, "mnu_Id", "Mnu_Name", "--Select Sub-Category--");
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            return View();
        }

        //
        // POST: /Admin/Create
        
        [HttpPost]
        [Authorize]
         public ActionResult Create(ProductInsert product)
        {
            //Check server side validation using data annotation
          //  var errors = ModelState
          //.Where(x => x.Value.Errors.Count > 0)
          //.Select(x => new { x.Key, x.Value.Errors })
          //.ToArray();
            if (ModelState.IsValid)
            {

                try
                {
                    
                   
                    // Insert Product
                    PRODUCT objnewproduct = new PRODUCT();
                    objnewproduct.Pid = (db.PRODUCTs.Max(i => (int?)i.Pid) ?? 0)+1;  //Convert.ToInt32(db.PRODUCTs.Max(x => x.Pid)) + 1;
                    objnewproduct.PDT_CODE = product.Product_Code;
                    objnewproduct.PDT_Name = product.Product_Name;
                    objnewproduct.PDT_Description = product.Product_Description;
                    objnewproduct.PDT_Price = product.Product_Price;
                    objnewproduct.Category_id = product.Category_id;
                    objnewproduct.Sub_Category_id =Convert.ToInt32(product.Sub_Category_id);
                    objnewproduct.offer_value_type_id = 1;// product.offer_id;
                    objnewproduct.Brand_id = Convert.ToInt32(product.brand_id);
                    objnewproduct.Add_user_Id = WebSecurity.CurrentUserId;
                    objnewproduct.Mod_user_id =  WebSecurity.CurrentUserId;
                    objnewproduct.Add_Datetime = System.DateTime.Now;
                    objnewproduct.Mod_Datetime = System.DateTime.Now;
                    objnewproduct.IsActive = true;
                    objnewproduct.PDT_offer = 1;// product.Product_offer;
                    objnewproduct.PDT_Discount_price = 1;// product.offer_id == 1 ? (product.Product_Price - (product.Product_offer * product.Product_Price) / 100) : product.Product_Price - product.Product_offer;
                    db.PRODUCTs.Add(objnewproduct);
                    db.SaveChanges();

                    int productid = db.PRODUCTs.Where(x => x.PDT_CODE == product.Product_Code).FirstOrDefault().Pid;
                    //insert PDT_IMAGE_PATH 
                    // save image in folder
                   //image1
                    bool Firstflag = false;
                    bool sideflag = false;
                    bool vertiflag = false;
                    bool horiflag = false;
                    if (product.Firstflag == 1)
                    {
                        Firstflag = true;
                    }
                    if (product.Firstflag == 2)
                    {
                        sideflag = true;
                    }
                    if (product.Firstflag == 3)
                    {
                        horiflag = true;
                    }
                    if (product.Firstflag == 4)
                    {
                        vertiflag = true;
                    }
                    string ImageName = System.IO.Path.GetFileName(product.Imagepath.FileName);
                    string physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                    product.Imagepath.SaveAs(physicalPath);
                    string targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                    product.Imagepath.SaveAs(targetPath);
                    PDT_IMAGE_PATH objImages = new PDT_IMAGE_PATH();
                    objImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                    objImages.PDT_ID = productid;
                    objImages.Image_Path = ImageName;                 
                    objImages.Home_Flag = product.Homeflag;
                    objImages.First_Flag = Firstflag;
                    objImages.Img_type = "F";
                    objImages.User_id = WebSecurity.CurrentUserId;
                    objImages.His_Datetime = System.DateTime.Now;
                    db.PDT_IMAGE_PATH.Add(objImages);

                    //sideimage
                    if (product.sideImagepath != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.sideImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.sideImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.sideImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objsideImages = new PDT_IMAGE_PATH();
                        objsideImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objsideImages.PDT_ID = productid;
                        objsideImages.Image_Path = ImageName;
                        objsideImages.First_Flag = sideflag;
                        objsideImages.Img_type = "S";
                        objsideImages.User_id = WebSecurity.CurrentUserId;
                        objsideImages.His_Datetime = System.DateTime.Now;
                        db.PDT_IMAGE_PATH.Add(objsideImages);
                    }

                    //Horizontal image
                    if (product.horizontalImagepath != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.horizontalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objhorzImages = new PDT_IMAGE_PATH();
                        objhorzImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objhorzImages.PDT_ID = productid;
                        objhorzImages.Image_Path = ImageName;                      
                        objhorzImages.User_id = WebSecurity.CurrentUserId;
                        objhorzImages.His_Datetime = System.DateTime.Now;
                        objhorzImages.First_Flag = horiflag;
                        objhorzImages.Img_type = "H";
                        db.PDT_IMAGE_PATH.Add(objhorzImages);
                    }
                    //vertical image
                    if (product.verticalImagepath != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.verticalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objvertImages = new PDT_IMAGE_PATH();
                        objvertImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objvertImages.PDT_ID = productid;
                        objvertImages.Image_Path = ImageName;                
                        objvertImages.User_id = WebSecurity.CurrentUserId;
                        objvertImages.His_Datetime = System.DateTime.Now;
                        objvertImages.First_Flag = vertiflag;
                        objvertImages.Img_type = "V";
                        db.PDT_IMAGE_PATH.Add(objvertImages);
                    }
                    //banner Image
                    if (product.BannerImagepath != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.BannerImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objBannerImages = new PDT_IMAGE_PATH();
                        objBannerImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objBannerImages.PDT_ID = productid;
                        objBannerImages.Image_Path = ImageName;
                        objBannerImages.User_id = WebSecurity.CurrentUserId;
                        objBannerImages.Img_type = "B";
                        objBannerImages.His_Datetime = System.DateTime.Now;
                        objBannerImages.Banner_Flag = true;
                        db.PDT_IMAGE_PATH.Add(objBannerImages);
                    }

                    db.SaveChanges();


                    //Specifications
                    foreach (var p in product.GetType().GetProperties())
                    {
                        // Console.WriteLine("Property {0} type {1} value {2}", p.Name, p.GetValue(obj, null).GetType().Name, p.GetValue(product, null));
                       
                        if (db.KEY_Specifications.Any(u => u.spec_col_Name == p.Name))
                        {
                            var propvalue = p.GetValue(product, null);
                            if (propvalue != null)
                            {
                            Value_Specification objvaluespec = new Value_Specification();
                            objvaluespec.Spec_value_id = (db.Value_Specification.Max(i => (int?)i.Spec_value_id) ?? 0) + 1;
                            objvaluespec.Product_id = productid;
                            objvaluespec.Key_spec_id = db.KEY_Specifications.Where(x => x.spec_col_Name == p.Name).FirstOrDefault().spec_col_id;                            
                            objvaluespec.key_Col_Value = propvalue.ToString();
                            objvaluespec.AddUser_id = WebSecurity.CurrentUserId;
                            objvaluespec.mod_user_Id = WebSecurity.CurrentUserId;
                            objvaluespec.Add_Datetime = System.DateTime.Now;
                            objvaluespec.Mod_Datetime = System.DateTime.Now;
                            db.Value_Specification.Add(objvaluespec);
                            db.SaveChanges();
                            }
                        }
                       



                    }

                    TempData["Message"] = ConfigurationManager.AppSettings["INS_SUC"];
                

                    ModelState.Clear();

                }

                catch(Exception exception ){
                    var queryError = from c in db.Menus
                                 where !(from o in db.menuTrees
                                         where o.Parent_Menuid != 0
                                         select o.Menu_id)
                                        .Contains(c.Mnu_id)
                                 select new { c.Mnu_id, c.Mnu_Name };
                    ViewBag.Menus = new SelectList(queryError, "mnu_Id", "Mnu_Name", "--Select Category--");
                    ViewBag.Brands = new SelectList(db.Brands.Where(act => act.IsActive == true), "BRND_id", "BRND_Name", "--Select Brand--");
                    ViewBag.Sub_category = new SelectList(db.Spec_category, "spec_cat_id", "spec_cat_Name", "--Select Sub--");
                    ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
                    return View(product);
                }
               
                return RedirectToAction("Create", "Admin");

            }
            var query = from c in db.Menus
                        where !(from o in db.menuTrees
                                where o.Parent_Menuid != 0
                                select o.Menu_id)
                               .Contains(c.Mnu_id)
                        select new { c.Mnu_id, c.Mnu_Name };
            ViewBag.Menus = new SelectList(query, "mnu_Id", "Mnu_Name", "--Select Category--");
            ViewBag.Brands = new SelectList(db.Brands.Where(act => act.IsActive == true), "BRND_id", "BRND_Name", "--Select Brand--");
            ViewBag.Sub_category = new SelectList(db.Spec_category, "spec_cat_id", "spec_cat_Name", "--Select Sub--");         
            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type");
            return View(product);
        }

        //
        // GET: /Admin/Edit/5
         [Authorize]
        public ActionResult Edit(int id = 0)
        {
          
          
            var  model = from p in db.PRODUCTs
                     join m in db.Menus on p.Category_id equals m.Mnu_id                    
                     join o in db.OFFER_VALUE_TYPE on p.offer_value_type_id equals o.OFR_VAl_Type_Id
                         join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                         from sbtype in db.Menus.Where(e => e.Mnu_id == p.Sub_Category_id).DefaultIfEmpty()
                         from brnd in db.Brands.Where(b => b.BRND_id == p.Brand_id).DefaultIfEmpty() 
                         where p.Pid == id && im.First_Flag == true  
                     select new 
                     {
                         Pid = p.Pid,
                         Product_Code = p.PDT_CODE,
                         Product_Name = p.PDT_Name,
                         Product_Description = p.PDT_Description,
                         Product_Price=p.PDT_Price,
                         Category_id=p.Category_id,
                         Sub_Category_id=p.Sub_Category_id,
                         offer_id=p.offer_value_type_id,
                         brand_id=p.Brand_id,
                         Imagepath=im.Image_Path,
                         Bannerflag=im.Banner_Flag,
                         Firstflag=im.First_Flag,
                         Homeflag=im.Home_Flag,
                         Product_offer=p.PDT_offer,
                         ImageType=im.Img_type
                     };
          
            EditProductdetails editprddetails = null;
            foreach (var m in model)
            {
                editprddetails = new EditProductdetails();
                editprddetails.Pid = m.Pid;
                editprddetails.Product_Code = m.Product_Code;
                editprddetails.Product_Name = m.Product_Name;
                editprddetails.Product_Description = m.Product_Description;
                editprddetails.Product_Price=m.Product_Price;
                editprddetails.Category_id=m.Category_id;
                editprddetails.Sub_Category_id=m.Sub_Category_id;
                editprddetails.offer_id=m.offer_id;
                //editprddetails.Imagepath=m.Imagepath;
                editprddetails.brand_id=m.brand_id;
                //editprddetails.Bannerflag=m.Bannerflag;
                editprddetails.Firstflag = Convert.ToInt32(m.Firstflag);
                //editprddetails.Homeflag=m.Homeflag;
                editprddetails.Product_offer = m.Product_offer;
            }
            if (model != null)
            {
                var sideimages = (from i in db.PDT_IMAGE_PATH
                                  where i.PDT_ID == id
                                  select new EditImageItem { ImgId = i.img_id, Imagepath = i.Image_Path, Imagetype = i.Img_type }).ToList();
                editprddetails.ImageList = sideimages;
            }
           
            var bannerimages = (from i in db.PDT_IMAGE_PATH
                              where i.PDT_ID == id && i.Banner_Flag==true
                              select new EditImageItem { ImgId = i.img_id, Imagepath = i.Image_Path, Imagetype = i.Img_type }).ToList();
            if (bannerimages.Count > 0)
            {
                editprddetails.BannerImageName = bannerimages.FirstOrDefault().Imagepath;
            }
            if (model != null)
            {
                var sideimages = (from i in db.PDT_IMAGE_PATH
                                  where i.PDT_ID == id && i.First_Flag==true
                                  select new  { i.Img_type }).ToList();
                foreach (var m in sideimages)
                {
                    switch (m.Img_type)
                    {
                        case "F":
                            editprddetails.Firstflag = 1;
                            break;
                        case "S":
                            editprddetails.Firstflag = 2;
                            break;
                        case "H":
                            editprddetails.Firstflag = 3;
                            break;
                        case "V":
                            editprddetails.Firstflag = 4;
                            break;
                        default:
                            editprddetails.Firstflag = 1;
                            break;
                    }
                }
               
            }
            var HomeFalgChk = (from i in db.PDT_IMAGE_PATH
                              where i.PDT_ID == id && i.Home_Flag == true
                              select new { i.Img_type }).ToList();
            foreach (var m in HomeFalgChk)
            {
                editprddetails.Homeflag = true;
            }
           

            foreach (var p in editprddetails.GetType().GetProperties())
            {
                // Console.WriteLine("Property {0} type {1} value {2}", p.Name, p.GetValue(obj, null).GetType().Name, p.GetValue(product, null));

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
                            editprddetails.GetType().GetProperty(p.Name).SetValue(editprddetails, s.key_Col_Value, null);
                        }
                    }
                }




            }

            var query = from c in db.Menus
                        where !(from o in db.menuTrees
                                where o.Parent_Menuid != 0
                                select o.Menu_id)
                               .Contains(c.Mnu_id)
                        select new { c.Mnu_id, c.Mnu_Name };
            ViewBag.Menus = new SelectList(query, "mnu_Id", "Mnu_Name", editprddetails.Category_id);
            ViewBag.Brands = new SelectList(db.Brands.Where(act => act.IsActive == true), "BRND_id", "BRND_Name", editprddetails.brand_id);

            ViewBag.offers = new SelectList(db.OFFER_VALUE_TYPE, "OFR_VAl_Type_Id", "Ofr_Value_type", editprddetails.offer_id);

            var subcat = (from c in db.Menus
                         where (from o in db.menuTrees
                                where o.Parent_Menuid != 0 && o.Parent_Menuid == editprddetails.Category_id
                                select o.Menu_id)
                                .Contains(c.Mnu_id)
                         select new { c.Mnu_id, c.Mnu_Name }).ToList();
            ViewBag.subcategory = new SelectList(subcat, "mnu_Id", "Mnu_Name", editprddetails.Sub_Category_id);

            return View(editprddetails);

        


         
           


            //PRODUCT product = db.PRODUCTs.Find(id);
            //if (product == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(product);
        }

        //
        // POST: /Admin/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditProductdetails product)
        {
            if (ModelState.IsValid)
            {


                //Update products
                var userToUpdateproducts = db.PRODUCTs.SingleOrDefault(u => u.Pid == product.Pid);
                if (userToUpdateproducts != null)
                {
                    userToUpdateproducts.PDT_Name = product.Product_Name;
                    //userToUpdateproducts.PDT_CODE = product.Product_Code;
                    userToUpdateproducts.PDT_Price = product.Product_Price;
                    userToUpdateproducts.PDT_Description = product.Product_Description;
                   // userToUpdateproducts.Category_id = product.Category_id;
                   // userToUpdateproducts.Sub_Category_id = Convert.ToInt32(product.Sub_Category_id);
                   // userToUpdateproducts.Brand_id = Convert.ToInt32(product.brand_id);
                   // userToUpdateproducts.offer_value_type_id = product.offer_id;
                    userToUpdateproducts.Mod_user_id =WebSecurity.CurrentUserId;
                    userToUpdateproducts.Mod_Datetime = System.DateTime.Now;
                   // userToUpdateproducts.PDT_offer = product.Product_offer;
                   // userToUpdateproducts.PDT_Discount_price = product.offer_id == 1 ? (product.Product_Price - (product.Product_offer * product.Product_Price) / 100) : product.Product_Price - product.Product_offer;
                    db.SaveChanges();
                }
                //Update PDT_image_Path table
                var userToUpdatepdt_img_path = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.First_Flag == true);
                if (userToUpdatepdt_img_path != null)
                {
                    userToUpdatepdt_img_path.First_Flag = false;
                    //userToUpdatepdt_img_path.Home_Flag = product.Homeflag;
                    userToUpdatepdt_img_path.User_id = WebSecurity.CurrentUserId;
                    userToUpdatepdt_img_path.His_Datetime = System.DateTime.Now;
                    db.SaveChanges();
                }
                var usrHomeFalg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Home_Flag == true);
                if (usrHomeFalg != null)
                {

                    usrHomeFalg.Home_Flag = product.Homeflag;
                    usrHomeFalg.User_id = WebSecurity.CurrentUserId;
                    usrHomeFalg.His_Datetime = System.DateTime.Now;
                    db.SaveChanges();
                }
                bool Firstflag = false;
                bool sideflag = false;
                bool vertiflag = false;
                bool horiflag = false;
                if (product.Firstflag == 1)
                {
                    Firstflag = true; 
                    var usrFirstFlag = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "F");
                    if (usrFirstFlag != null)
                    {
                        usrFirstFlag.First_Flag = Firstflag;
                        usrFirstFlag.User_id = WebSecurity.CurrentUserId;
                        usrFirstFlag.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }
                if (product.Firstflag == 2)
                {
                    sideflag = true;
                    var usrsideFlag = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "S");
                    if (usrsideFlag != null)
                    {
                        usrsideFlag.First_Flag = sideflag;
                        usrsideFlag.User_id = WebSecurity.CurrentUserId;
                        usrsideFlag.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }
                if (product.Firstflag == 3)
                {
                    horiflag = true;
                    var usrhoriFlag = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "H");
                    if (usrhoriFlag != null)
                    {
                        usrhoriFlag.First_Flag = horiflag;
                        usrhoriFlag.User_id = WebSecurity.CurrentUserId;
                        usrhoriFlag.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }
                if (product.Firstflag == 4)
                {
                    vertiflag = true;
                    var usrvertiFlag = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "V");
                    if (usrvertiFlag != null)
                    {
                        usrvertiFlag.First_Flag = vertiflag;
                        usrvertiFlag.User_id = WebSecurity.CurrentUserId;
                        usrvertiFlag.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }

               
                //banner Image
                string ImageName, physicalPath, targetPath;
                if (product.BannerImagepath != null)
                {
                    var updBannerImg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Banner_Flag == true);
                    if (updBannerImg != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.BannerImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(targetPath);
                        updBannerImg.Banner_Flag = true;
                        updBannerImg.Image_Path = ImageName;                       
                        updBannerImg.User_id = WebSecurity.CurrentUserId;
                        updBannerImg.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        ImageName = System.IO.Path.GetFileName(product.BannerImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.BannerImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objBannerImages = new PDT_IMAGE_PATH();
                        objBannerImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objBannerImages.PDT_ID = product.Pid;
                        objBannerImages.Image_Path = ImageName;
                        objBannerImages.User_id = WebSecurity.CurrentUserId;
                        objBannerImages.Img_type = "B";
                        objBannerImages.His_Datetime = System.DateTime.Now;
                        objBannerImages.Banner_Flag = true;
                        db.PDT_IMAGE_PATH.Add(objBannerImages);
                    }
                }
                //sideImage
                if (product.sideImagepath != null)
                {
                    //if exists update the new image
                    var updsideImg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "S");
                    if (updsideImg != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.sideImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.sideImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.sideImagepath.SaveAs(targetPath);
                        updsideImg.Image_Path = ImageName;
                        updsideImg.First_Flag = sideflag;
                        updsideImg.User_id = WebSecurity.CurrentUserId;
                        updsideImg.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                    //if does not exist insert the new image
                    else
                    {
                        ImageName = System.IO.Path.GetFileName(product.sideImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.sideImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.sideImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objsideImages = new PDT_IMAGE_PATH();
                        objsideImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objsideImages.PDT_ID = product.Pid;
                        objsideImages.Image_Path = ImageName;
                        objsideImages.First_Flag = sideflag;
                        objsideImages.Img_type = "S";
                        objsideImages.User_id = WebSecurity.CurrentUserId;
                        objsideImages.His_Datetime = System.DateTime.Now;
                        db.PDT_IMAGE_PATH.Add(objsideImages);
                    }
                }

                //verticalImage
                if (product.verticalImagepath != null)
                {
                    var updvertiImg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "V");
                    if (updvertiImg != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.verticalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(targetPath);
                        updvertiImg.Image_Path = ImageName;
                        updvertiImg.First_Flag = vertiflag;
                        updvertiImg.User_id = WebSecurity.CurrentUserId;
                        updvertiImg.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        ImageName = System.IO.Path.GetFileName(product.verticalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.verticalImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objvertImages = new PDT_IMAGE_PATH();
                        objvertImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objvertImages.PDT_ID = product.Pid;
                        objvertImages.Image_Path = ImageName;
                        objvertImages.User_id = WebSecurity.CurrentUserId;
                        objvertImages.His_Datetime = System.DateTime.Now;
                        objvertImages.First_Flag = vertiflag;
                        objvertImages.Img_type = "V";
                        db.PDT_IMAGE_PATH.Add(objvertImages);
                    }
                }
                //HorizontalImage
                if (product.horizontalImagepath != null)
                {
                    var updhoriImg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "H");
                    if (updhoriImg != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.horizontalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(targetPath);
                        updhoriImg.Image_Path = ImageName;
                        updhoriImg.First_Flag = horiflag;
                        updhoriImg.User_id = WebSecurity.CurrentUserId;
                        updhoriImg.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        ImageName = System.IO.Path.GetFileName(product.horizontalImagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.horizontalImagepath.SaveAs(targetPath);
                        PDT_IMAGE_PATH objhorzImages = new PDT_IMAGE_PATH();
                        objhorzImages.img_id = (db.PDT_IMAGE_PATH.Max(i => (int?)i.img_id) ?? 0) + 1;
                        objhorzImages.PDT_ID = product.Pid;
                        objhorzImages.Image_Path = ImageName;
                        objhorzImages.User_id = WebSecurity.CurrentUserId;
                        objhorzImages.His_Datetime = System.DateTime.Now;
                        objhorzImages.First_Flag = horiflag;
                        objhorzImages.Img_type = "H";
                        db.PDT_IMAGE_PATH.Add(objhorzImages);
                    }
                }
                //First Image
                if (product.Imagepath != null)
                {
                    var updFirstImg = db.PDT_IMAGE_PATH.SingleOrDefault(u => u.PDT_ID == product.Pid && u.Img_type == "F");
                    if (updFirstImg != null)
                    {
                        ImageName = System.IO.Path.GetFileName(product.Imagepath.FileName);
                        physicalPath = Server.MapPath(ConfigurationManager.AppSettings["ORIGINALPATH"] + ImageName);
                        product.Imagepath.SaveAs(physicalPath);
                        targetPath = Server.MapPath(ConfigurationManager.AppSettings["RESZEFILEPATH"] + ImageName);
                        product.Imagepath.SaveAs(targetPath);
                        updFirstImg.Image_Path = ImageName;
                        updFirstImg.First_Flag = Firstflag;
                        updFirstImg.User_id = WebSecurity.CurrentUserId;
                        updFirstImg.His_Datetime = System.DateTime.Now;
                        db.SaveChanges();
                    }
                }

                //subscription

                //Add new Specifications 
                foreach (var p in product.GetType().GetProperties())
                {
                    // Console.WriteLine("Property {0} type {1} value {2}", p.Name, p.GetValue(obj, null).GetType().Name, p.GetValue(product, null));

                    if (db.KEY_Specifications.Any(u => u.spec_col_Name == p.Name))
                    {
                         var chkdescription = from v in db.Value_Specification
                                       join K in db.KEY_Specifications on v.Key_spec_id equals K.spec_col_id
                                       where v.Product_id == product.Pid && K.spec_col_Name==p.Name
                                       select new { K.spec_col_Name, v.key_Col_Value,v.Spec_value_id };

                         if (!chkdescription.Any())
                        {
                            if (p.GetValue(product)!=null)
                            {
                                Value_Specification objvaluespec = new Value_Specification();
                                objvaluespec.Spec_value_id = (db.Value_Specification.Max(i => (int?)i.Spec_value_id) ?? 0) + 1;
                                objvaluespec.Product_id = product.Pid;
                                objvaluespec.Key_spec_id = db.KEY_Specifications.Where(x => x.spec_col_Name == p.Name).FirstOrDefault().spec_col_id;
                                objvaluespec.key_Col_Value = p.GetValue(product, null).ToString();
                                objvaluespec.AddUser_id =  WebSecurity.CurrentUserId;
                                objvaluespec.mod_user_Id =  WebSecurity.CurrentUserId;
                                objvaluespec.Add_Datetime = System.DateTime.Now;
                                objvaluespec.Mod_Datetime = System.DateTime.Now;
                                db.Value_Specification.Add(objvaluespec);
                                db.SaveChanges();
                            }
                        }
                    }



                }
                 //Edit old Specifications 

                foreach (var p in product.GetType().GetProperties())
                {
                    // Console.WriteLine("Property {0} type {1} value {2}", p.Name, p.GetValue(obj, null).GetType().Name, p.GetValue(product, null));

                    var subscription = from v in db.Value_Specification
                                       join K in db.KEY_Specifications on v.Key_spec_id equals K.spec_col_id
                                       where v.Product_id == product.Pid
                                       select new { K.spec_col_Name, v.key_Col_Value,v.Spec_value_id };

                    if (subscription.Any(u => u.spec_col_Name == p.Name))
                    {
                        foreach (var s in subscription)
                        {
                            if (s.spec_col_Name == p.Name)
                            {
                                var usrspecificationtoupd = db.Value_Specification.SingleOrDefault(u => u.Spec_value_id == s.Spec_value_id);
                                if (usrspecificationtoupd != null)
                                {
                                    usrspecificationtoupd.key_Col_Value = p.GetValue(product, null).ToString();
                                    usrspecificationtoupd.mod_user_Id = WebSecurity.CurrentUserId;
                                    usrspecificationtoupd.Mod_Datetime = System.DateTime.Now;
                                   
                                }
                               
                               
                            }
                        }
                    }




                }
                db.SaveChanges();

                TempData["Message"] = ConfigurationManager.AppSettings["EDT_SUC"];
                return RedirectToAction("Index");
              
            }
            return View(product);

            
        }

        //
        // GET: /Admin/Delete/5
         [Authorize]
        public ActionResult Delete(int id = 0)
        {
           
           var model = from p in db.PRODUCTs
                    join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                    where im.First_Flag == true
                    select new deleteProductdetails
                    {
                        Pid = p.Pid,
                        Product_Code = p.PDT_CODE,
                        Product_Name = p.PDT_Name,
                        ImagePath = im.Image_Path

                    };
           deleteProductdetails delprddetails = null;
           foreach (var m in model)
           {
               delprddetails = new deleteProductdetails();
               delprddetails.Pid = m.Pid;
               delprddetails.Product_Code = m.Product_Code;
               delprddetails.Product_Name = m.Product_Name;
               delprddetails.ImagePath = m.ImagePath;
           }

           return View(delprddetails);
            
        }

        //
        // POST: /Admin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            var userToUpdate = db.PRODUCTs.SingleOrDefault(u => u.Pid == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = false;
                userToUpdate.Mod_user_id = WebSecurity.CurrentUserId;
                userToUpdate.Mod_Datetime = System.DateTime.Now;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ReActivate(int id = 0)
        {

            var model = from p in db.PRODUCTs
                        join im in db.PDT_IMAGE_PATH on p.Pid equals im.PDT_ID
                        where im.First_Flag == true
                        select new deleteProductdetails
                        {
                            Pid = p.Pid,
                            Product_Code = p.PDT_CODE,
                            Product_Name = p.PDT_Name,
                            ImagePath = im.Image_Path

                        };
            deleteProductdetails delprddetails = null;
            foreach (var m in model)
            {
                delprddetails = new deleteProductdetails();
                delprddetails.Pid = m.Pid;
                delprddetails.Product_Code = m.Product_Code;
                delprddetails.Product_Name = m.Product_Name;
                delprddetails.ImagePath = m.ImagePath;
            }

            return View(delprddetails);

        }
        [HttpPost, ActionName("ReActivate")]
        [ValidateAntiForgeryToken]
        public ActionResult ReActivateConfirmed(int id)
        {

            var userToUpdate = db.PRODUCTs.SingleOrDefault(u => u.Pid == id);
            if (userToUpdate != null)
            {

                userToUpdate.IsActive = true;
                userToUpdate.Mod_user_id = WebSecurity.CurrentUserId;
                userToUpdate.Mod_Datetime = System.DateTime.Now;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
        public JsonResult LoadSubCategory(string menuid)
        {
            int mid = Convert.ToInt32(menuid);
            var query = (from c in db.Menus
                         where (from o in db.menuTrees
                                where o.Parent_Menuid != 0 && o.Parent_Menuid == mid
                                select o.Menu_id)
                                .Contains(c.Mnu_id)
                         select new { c.Mnu_id, c.Mnu_Name }).ToList();
            ViewBag.Sub_category = new SelectList(query, "mnu_Id", "Mnu_Name", "--Sub Category--");
            return Json(query);
        }
        public JsonResult Loadbrands(string subcatid,string cat)
        {
            int sid = Convert.ToInt32(subcatid);
            int catid = Convert.ToInt32(cat);

            var query = (from c in db.Brands
                         where c.cat_id==catid && c.sub_cat_id == sid && c.IsActive==true
                         select new { c.BRND_id, c.BRND_Name }).ToList();
            ViewBag.Sub_category = new SelectList(query, "BRND_id", "BRND_Name", "--brands--");
            return Json(query);
           
            //if (cat == "A")
            //{
            //    var query = (from c in db.Brands
            //             where c.sub_cat_id == sid
            //             select new { c.BRND_id, c.BRND_Name }).ToList();
            //    ViewBag.Sub_category = new SelectList(query, "BRND_id", "BRND_Name", "--brands--");
            //    return Json(query);

            //}
            //else
            //{
            //    var query = (from c in db.Brands
            //             where c.cat_id == sid
            //             select new { c.BRND_id, c.BRND_Name }).ToList();
            //    ViewBag.Sub_category = new SelectList(query, "BRND_id", "BRND_Name", "--brands--");
            //    return Json(query);
            //}
           
        }
        public JsonResult CheckPDTExists(string pdt_code)
        {
            
            return Json(db.PRODUCTs.Any(u => u.PDT_CODE == pdt_code));
        }


    }
}