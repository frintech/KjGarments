using KJGarmentsWeb.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class BannerImageModel
    {
    }

    public class AddBannerImageModel
    {
        //Image path details
        [Required(ErrorMessage = "Please select your image")]
        [NotMapped]
        [ValidateFile]
        public HttpPostedFileBase Imagepath { get; set; }
      
    }
    public class ViewBannerImageModel
    {
        public bool Active { get; set; }       
        public long BNRImgId { get; set; }
        public string Imagepath { get; set; }
    }
   
    public class deleteBannerImageModel
    {
        public long BNRImgId { get; set; }
        public string Imagepath { get; set; }


    }
}