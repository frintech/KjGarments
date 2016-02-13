using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KJGarmentsWeb.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KJGarmentsWeb.Models
{
    public class CouponModel
    {
     
        //treeview 
            public int id { get; set; }
            public Nullable<int> menu_id { get; set; }
            public Nullable<int> Parent_Menuid { get; set; }
            public string origin { get; set; }
            public string Item_Name { get; set; }
            public string parent_name { get; set; }
            public IList<CouponModel> tvlists { set; get; }
            public CouponModel()
            {
                tvlists = new List<CouponModel>();
            }

        //special Coupon
         [Required(ErrorMessage = "Please enter  Coupon Code")]
         public string cupn_Code { get; set; }
         [Required(ErrorMessage = "Please enter Coupon Name")]
         public string cupn_Name { get; set; }
        public string Description { get; set; }
         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter  StartDate")]
         public DateTime StartDate { get; set; }
         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter EndDate")]
          public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Please enter  User Commision")]
         public Nullable<int> MinQty { get; set; }
         [Required(ErrorMessage = "Please select  offer type")]
         public Nullable<int> MaxQty { get; set; }
          
         public Nullable<int> Discountpct { get; set; }
         public int offer_id { get; set; }
         public int userid { get; set; }
        
    }

    public class ViewCouponModel
    {
        public string cupn_Code { get; set; }
        public string cupn_Name { get; set; }
        public string Description { get; set; }      
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Nullable<int> MinQty { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public Nullable <bool> Isactive { get; set; }
        public int couponid { get; set; }
        public string usercode { get; set; }
    }
    public class deleteCouponModel
    {
        public int couponid { get; set; }
        public string cupn_Code { get; set; }
        public string cupn_Name { get; set; }


    }
    public class EditCouponModel
    {

        //treeview 
        public int id { get; set; }
        public int menu_id { get; set; }
        public int Parent_Menuid { get; set; }
        public string origin { get; set; }
        public string Item_Name { get; set; }
        public string parent_name { get; set; }
        public IList<EditCouponModel> tvlists { set; get; }
        public EditCouponModel()
        {
            tvlists = new List<EditCouponModel>();
        }

        // Coupon
         
        public string Coupon_Code { get; set; }
          [Required(ErrorMessage = "Please enter Coupon Name")]
        public string Coupon_Name { get; set; }
        public string Description { get; set; }
         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter  StartDate")]
        public DateTime StartDate { get; set; }
         [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please enter EndDate")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Please enter  User Commision")]
        public Nullable<int> MinQty { get; set; }
        [Required(ErrorMessage = "Please select  offer type")]
        public Nullable<int> MaxQty { get; set; } 
        public Nullable<decimal> Discountpct { get; set; }
        public Nullable<int> offer_id { get; set; }
        public int Couponid { get; set; }
        public int userid { get; set; }

    }
}