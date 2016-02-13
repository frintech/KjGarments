using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class AddBrandModel
    {
        [Required(ErrorMessage = "Please enter your product Code")]
        public string BRND_Name { get; set; }
        [Required(ErrorMessage = "Please enter your product Code")]
        public string BRND_Description { get; set; }
         public HttpPostedFileBase BRND_Img_Path { get; set; }
        public bool Is_Add_img { get; set; }       
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Please select Category")]
        public int Category_id { get; set; }
        public Nullable<int> Sub_Category_id { get; set; }
    }
    public class ViewBrandModel
    {
        public string BRND_Name { get; set; }
        public string BRND_Description { get; set; }
        public string BRND_Img_Path { get; set; }
        public bool Is_Add_img { get; set; }
        public bool IsActive { get; set; }
        public string category_name { get; set; }
        public string sub_category_name { get; set; }
        public int brndId { get; set; }
    }
    public class EditBrandModel
    {
        [Required(ErrorMessage = "Please enter your product Code")]
        public string BRND_Name { get; set; }
        [Required(ErrorMessage = "Please enter your product Code")]
        public string BRND_Description { get; set; }
        public HttpPostedFileBase BRND_Img_Path { get; set; }
        public bool Is_Add_img { get; set; }
        public bool IsActive { get; set; }       
        public Nullable<int> Category_id { get; set; }
        public Nullable<int> Sub_Category_id { get; set; }
        public int brndId { get; set; }
        public string BRND_Img_Path_name { get; set; }
    }
    public class deleteBrandModel
    {
        public int brndId { get; set; }
        public string BRND_Name { get; set; }
        public string BRND_Description { get; set; }
       

    }

}