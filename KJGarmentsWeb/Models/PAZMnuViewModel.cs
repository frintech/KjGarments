using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KJGarmentsWeb.DAL;

namespace KJGarmentsWeb.Models
{
    public class PAZMnuViewModel
    {
        public string CategoryName { get; set; }      
        public string ImagePath  { get; set; }
        public int categoryid { get; set; }
        public int grpid { get; set; }
    }
    public class ImageDetailsModel
    {
        public int ImageId { get; set; }
        public string ImageTitle { get; set; }
        public string ImageDescription { get; set; }
        public string ImagePath { get; set; }
        public string ArtistName { get; set; }
        public string CategoryName { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<decimal> Price { get; set; }
    }

    public class CatDetailsViewModel
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public int Imgid { get; set; }
        public int categoryid { get; set; }
        public int grpid { get; set; }
        public string ImageDesc { get; set; }
        public string categoryName { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int quantity { get; set; }
    }

    public class MyModelCategory
    {
        public int CategoryId { get; set; }
        public IEnumerable<CatList> Categories { get; set; }
        public int ArtistId { get; set; }
    }
    

    public class CatList
    {
        [Required (ErrorMessage="Please Select the Category name")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ArtistId { get; set; }
        [Required]
        public string ImageTitile { get; set; }
         [Required]
        public string ImageDesc { get; set; }
         [Required]
        public string ImageFilePath { get; set; }        
        [Required]
         public Nullable<decimal> Price { get; set; }

    }

    //public class CategoryImagdetailsList 
    //{
    //   public ImageDetail Imageitems {get; set;}
    //   public MyModelCategory CatItems {get; set;}
    //}

    public class artistDetailsViewModel
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public int ImgId { get; set; }
        public int ArtistsId { get; set; }
        public int grpid { get; set; }
        public string ImageDesc { get; set; }
        public string ArtistsName { get; set; }
        public Nullable<decimal> Price { get; set; }
    }
    public class CategoryViewDetailsModel
    {
        public int catid { get; set; }
        public string CategoryName { get; set; }
        public string GroupName { get; set; }       
        public Nullable<bool> Active { get; set; }
    }
    public class ViewProductdetails
    {
        public int Pid { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public decimal Product_Price { get; set; }
        public string Product_Description { get; set; }
        public string Category { get; set; }
        public bool Isactive { get; set; }
        public string Sub_Category { get; set; }
        public string offer { get; set; }
        public string brand { get; set; }
        public string ImagePath { get; set; }
        public decimal Product_offer { get; set; }
    }
    public class deleteProductdetails
    {
        public int Pid { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public decimal Product_Price { get; set; }        
        public string ImagePath { get; set; }
        
    }
    public class EditImageItem
    {
        public int ImgId { get; set; }
        public string Imagetype { get; set; }
        public string Imagepath { get; set; }
    }
    public class BannerImagedetails
    {
        public long ImgId { get; set; }    
        public string Imagepath { get; set; }
    }
    public class EditProductdetails
   {
     //Product details
        public int Pid { get; set; }
         [Required(ErrorMessage = "Please enter your product Code")]
        public string Product_Code { get; set; }
         [Required(ErrorMessage = "Please enter your product Name")]
         [StringLength(60, MinimumLength = 3)]
        public string Product_Name { get; set; }
         [Required(ErrorMessage = "Please enter Price Value")]        
        public decimal Product_Price { get; set; }
         [Required(ErrorMessage = "Please enter Product Description")]
         public string Product_Description { get; set; }
         [Required(ErrorMessage = "Please select Category")]
        public int Category_id { get; set; }
        public int? Sub_Category_id { get; set; }
        public int offer_id { get; set; }
        public int? brand_id { get; set; }
        
        public HttpPostedFileBase Imagepath { get; set; }
        [Required(ErrorMessage = "Please enter offer Value")]
        public decimal Product_offer { get; set; }
        public HttpPostedFileBase BannerImagepath { get; set; }
        public virtual ICollection<EditImageItem> ImageList { get; set; }
        //public string Imagepath { get; set; }
        public HttpPostedFileBase sideImagepath { get; set; }
        public HttpPostedFileBase horizontalImagepath { get; set; }
        public HttpPostedFileBase verticalImagepath { get; set; }
        public bool Homeflag { get; set; }
        //public bool Bannerflag { get; set; }
        [Required(ErrorMessage = "Please select First Flag")]
        public int Firstflag { get; set; }
        public string ImageType { get; set; }
        public string BannerImageName { get; set; }
        // Product Description
        [Required(ErrorMessage = "Please enter your Model")]
        [StringLength(60, MinimumLength = 3)]
          public string Brand { get; set; }         
          public string Model { get; set; }
          public string Movement { get; set; }
          public string Features { get; set; }
          public string Water_Resistance { get; set; }
          public string Box { get; set; }
          public string Case_Material { get; set; }
          public string Case_Back { get; set; }
          public string Case_Size { get; set; }
          public string Glass { get; set; }
          public string Dial_Colour { get; set; }
          public string Strap_Type { get; set; }
          public string Warranty { get; set; }
          public string Shipping { get; set; }
          public string Payment_Method { get; set; }
        //SunGalsses
          public string Lens_Type { get; set; }
          public string Lens_Colour { get; set; }
         public string Frame_Colour { get; set; }

         //Luxury Pens
         public string Product_Type { get; set; }
         public string Pen_Colour { get; set; }

         //Belts
         public string Belt_Size { get; set; }
         public string Buckle_Colour { get; set; }
         public string Belt_Colour { get; set; }
         //wallets
         public string Wallet_Colour { get; set; }
         
     }
    public class ProductdetailsView
    {
        //Product details
        public int Pid { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public decimal Product_Price { get; set; }
        public string Product_Description { get; set; }
        public int Category_id { get; set; }
        public int Sub_Category_id { get; set; }
        public int offer_id { get; set; }
        public int brand_id { get; set; }
        public string Imagepath { get; set; }
        public decimal Product_offer { get; set; }
        public decimal? discount_price { get; set; }
        public string Coupon_Code { get; set; }
        public string offer_type { get; set; }
        public virtual ICollection<string> ImageList { get; set; }
        public string sideImagepath { get; set; }
        public string horizontalImagepath { get; set; }
        public string verticalImagepath { get; set; }
        public bool Homeflag { get; set; }
        public bool Bannerflag { get; set; }
        public bool Firstflag { get; set; }
        public bool Isactive { get; set; }
        // Product Description
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Movement { get; set; }
        public string Features { get; set; }
        public string Water_Resistance { get; set; }
        public string Box { get; set; }
        public string Case_Material { get; set; }
        public string Case_Back { get; set; }
        public string Case_Size { get; set; }
        public string Glass { get; set; }
        public string Dial_Colour { get; set; }
        public string Strap_Type { get; set; }
        public string Warranty { get; set; }
        public string Shipping { get; set; }
        public string Payment_Method { get; set; }
        //SunGalsses
        public string Lens_Type { get; set; }
        public string Lens_Colour { get; set; }
        public string Frame_Colour { get; set; }

        //Luxury Pens
        public string Product_Type { get; set; }
        public string Pen_Colour { get; set; }

        //Belts
        public string Belt_Size { get; set; }
        public string Buckle_Colour { get; set; }
        public string Belt_Colour { get; set; }
        //wallets
        public string Wallet_Colour { get; set; }
    }
    public class MnuSubcatProductDetails
    {
        public int Pid { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }        
        public string ProductDesc { get; set; }
        
        public Nullable<decimal> Price { get; set; }
        public string offer { get; set; }
        public Nullable<decimal> Product_Offer { get; set; }
        public string Imagepath { get; set; }
        public virtual ICollection<EditImageItem> BannerList { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }
        public int brandid { get; set; }
        public int subcatid { get; set; }
        public int catid { get; set; }
        
    }
    public class ViewofferDetails
    {
        public int Pid { get; set; }
        public int Offer { get; set; }
    }
    public class MenuBrandView
    {
        public int MnuBrnandId { get; set; }
        public string Brandname { get; set; }
        public int catid { get; set; }
        public int Subcatid { get; set; }
        public string ImagePath { get; set; }
    }
   
}