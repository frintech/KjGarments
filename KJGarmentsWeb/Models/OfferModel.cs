using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KJGarmentsWeb.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KJGarmentsWeb.Models
{
    public class OfferModel
    {
     
        //treeview 
            public int id { get; set; }
            public Nullable<int> menu_id { get; set; }
            public Nullable<int> Parent_Menuid { get; set; }
            public string origin { get; set; }
            public string Item_Name { get; set; }
            public string parent_name { get; set; }
            public IList<OfferModel> tvlists { set; get; }
            public OfferModel()
            {
                tvlists = new List<OfferModel>();
            }

        //special offer
         [Required(ErrorMessage = "Please enter  offer Code")]
         public string offer_Code { get; set; }
         [Required(ErrorMessage = "Please enter Offer Name")]
          public string offer_Name { get; set; }
        public string Description { get; set; }         
        public string Type { get; set; }
       
        public string usr_Category { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter  StartDate")]
         public DateTime StartDate { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter EndDate")]
          public DateTime EndDate { get; set; }
        [Range(1,100,ErrorMessage ="Please enter  values with 1 to 100")]
        [Required(ErrorMessage = "Please enter  MinQty")]
         public Nullable<int> MinQty { get; set; }
         [Range(1, 100, ErrorMessage = "Please enter  values with 1 to 100")]
         [Required(ErrorMessage = "Please enter  MaxQty")]
         public Nullable<int> MaxQty { get; set; }
         public HttpPostedFileBase AddBannerImgpath { get; set; }   
         public Nullable<int> Discountpct { get; set; }         
         public int offer_id { get; set; }

        
    }

    public class ViewofferModel
    {
        public string offer_Code { get; set; }
        public string offer_Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string usr_Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Nullable<int> MinQty { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public Nullable <bool> Isactive { get; set; }
        public int SpecialOfferID { get; set; }
    }
    public class deleteOfferModel
    {
        public int SpecialOfferID { get; set; }
        public string offer_Code { get; set; }
        public string offer_Name { get; set; }


    }
    public class EditOfferModel
    {

        //treeview 
        public int id { get; set; }
        public Nullable<int> menu_id { get; set; }
        public Nullable<int> Parent_Menuid { get; set; }
        public string origin { get; set; }
        public string Item_Name { get; set; }
        public string parent_name { get; set; }
        public IList<EditOfferModel> tvlists { set; get; }
        public EditOfferModel()
        {
            tvlists = new List<EditOfferModel>();
        }

        //special offer
        public string offer_Code { get; set; }
              [Required(ErrorMessage = "Please enter Offer Name")]
        public string offer_Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string usr_Category { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
           [Required(ErrorMessage = "Please enter  StartDate")]
        public DateTime StartDate { get; set; }
         [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
           [Required(ErrorMessage = "Please enter EndDate")]
        public DateTime EndDate { get; set; }
           [Required(ErrorMessage = "Please enter  MinQty")]
        public Nullable<int> MinQty { get; set; }
         [Required(ErrorMessage = "Please enter  MaxQty")]
        public Nullable<int> MaxQty { get; set; }
        public HttpPostedFileBase AddBannerImgpath { get; set; }
        public Nullable<decimal> Discountpct { get; set; }
        public Nullable<int> offer_id { get; set; }
        public int splofferid { get; set; }
        public string AddBnrimageName { get; set; }

    }
}