using KJGarmentsWeb.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
namespace KJGarmentsWeb.DAL
{
    public class AdminModel
    {
    }
   // [AtLeastOneProperty("Brand", "Model", "Movement", "Features", ErrorMessage = "You must enter at least one value")]
    
    public class ProductInsert
    {
       
        //Product details
        [Required(ErrorMessage = "Please enter your product Code")]
        public string Product_Code { get; set; }
        [Required(ErrorMessage = "Please enter your product Name")]
        [StringLength(60, MinimumLength = 3)]
        public string Product_Name { get; set; }
        [Required(ErrorMessage = "Please enter Price Value")]
        [Range(1, 10000000), DataType(DataType.Currency)]
        public decimal Product_Price { get; set; }
         [Required(ErrorMessage = "Please enter Product Description")]
        public string Product_Description { get; set; }
        [Required(ErrorMessage = "Please select Category")]
        public int Category_id { get; set; }     
        public int? Sub_Category_id { get; set; }
        public int offer_id { get; set; }
        public int? brand_id { get; set; }
        //[Required(ErrorMessage = "Please enter offer Value")]
        public decimal Product_offer { get; set; }
        //Image path details
        [Required(ErrorMessage = "Please select your image")]       
        [NotMapped]
        [ValidateFile]
        public HttpPostedFileBase Imagepath { get; set; }
        
        public HttpPostedFileBase sideImagepath { get; set; }
       
        public HttpPostedFileBase horizontalImagepath { get; set; }
       
        public HttpPostedFileBase verticalImagepath { get; set; }
        public HttpPostedFileBase BannerImagepath { get; set; }
        public bool Homeflag { get; set; }
        //public bool Bannerflag { get; set; }
        [Required(ErrorMessage = "Please select First Flag")]
        public int Firstflag { get; set; }

      ////  public bool SideHomeflag { get; set; }
      //  public bool SideBannerflag { get; set; }
      //  public bool SideFirstflag { get; set; }

      // // public bool HoriHomeflag { get; set; }
      //  public bool HoriBannerflag { get; set; }
      //  public bool HoriFirstflag { get; set; }

      // // public bool vertiHomeflag { get; set; }
      //  public bool vertiBannerflag { get; set; }
      //  public bool vertiFirstflag { get; set; }
        // Product Description

        [Required(ErrorMessage = "Please enter your Brand")]
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AtLeastOnePropertyAttribute : ValidationAttribute
    {
        private string[] PropertyList { get; set; }

        public AtLeastOnePropertyAttribute(params string[] propertyList)
        {
            this.PropertyList = propertyList;
        }

       
        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        public override bool IsValid(object value)
        {
            PropertyInfo propertyInfo;
            foreach (string propertyName in PropertyList)
            {
                propertyInfo = value.GetType().GetProperty(propertyName);

                if (propertyInfo != null && propertyInfo.GetValue(value, null) != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

    




}