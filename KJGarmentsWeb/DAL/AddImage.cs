using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KJGarmentsWeb.DAL
{
   public class ImageInfo
    {

        [Required(ErrorMessage = "Please Select your Category ! ")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        //[Required(ErrorMessage = "Please Select your Artist Titile ! ")]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Please enter your Image Titile ! ")]

        [StringLength(25)]

        public string ImageTitile { get; set; }



        [Required(ErrorMessage = "Please enter your Image Description !")]

        [StringLength(25)]

        public string ImageDesc { get; set; }

        [Required(ErrorMessage = "Please enter Price !")]
        //[RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "Price value should only numerics")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public Nullable<decimal> Price { get; set; }

        [Required(ErrorMessage = "Please browse your image")]

        [Display(Name = "Upload Image")]

        [NotMapped]

        [ValidateFile]

        public HttpPostedFileBase ImagePhoto { get; set; }

    }
    public class ValidateFileAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {

            int maxContent = 1024 * 1024; //1 MB

            string[] sAllowedExt = new string[] { ".jpg", ".gif", ".png", ".JPG", ".JPEG" };


            var file = value as HttpPostedFileBase;



            if (file == null)

                return false;

            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {

                ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", sAllowedExt);

                return false;

            }

            else if (file.ContentLength > maxContent)
            {

                ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (maxContent / 1024).ToString() + "MB";

                return false;

            }

            else

                return true;

        }

    }
}