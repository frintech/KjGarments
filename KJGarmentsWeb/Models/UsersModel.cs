using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class AddUserModel
    {
        [Required(ErrorMessage = "Please enter your First Name")]
        public string First_name { get; set; }
        [Required(ErrorMessage = "Please enter your Last name")]
        public string Last_name { get; set; }
        [Required(ErrorMessage = "Please enter your Mobile No")]
        public string Mobile_number { get; set; } 
        [EmailAddress]
        [Required(ErrorMessage = "Please enter your Emial ID ")]
        public string Email_id { get; set; }
        [Required(ErrorMessage = "Please select DOB")]
        public DateTime DOB { get; set; }
         [Required(ErrorMessage = "Please enter your Address ")]
        public string Address_Communication { get; set; }
       
               
         [Required(ErrorMessage = "Please select your Gender ")]
        public string Gender { get; set; }

         [Required(ErrorMessage = "Please enter your Password ")]
         [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "New password")]
         public string Usr_pwd { get; set; }

         [DataType(DataType.Password)]
         [Display(Name = "Confirm new password")]
         [Compare("Usr_pwd", ErrorMessage = "The new password and confirmation password do not match.")]
         public string ConfirmPassword { get; set; }





    }
    public class ViewUserModel
    {
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Address { get; set; }
        public string Email_id { get; set; }
        public DateTime DOB { get; set; }      
        public string Mobile_number { get; set; }
        public string Gender { get; set; }
        public string user_type { get; set; }
        public Nullable<bool> Isactive { get; set; }
        public int Uidno { get; set; }
    }
    public class EditUserModel
    {
        [Required(ErrorMessage = "Please enter your First Name")]
        public string First_name { get; set; }
        [Required(ErrorMessage = "Please enter your Last name")]
        public string Last_name { get; set; }
        [Required(ErrorMessage = "Please enter your Mobile No")]
        public string Mobile_number { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter your Emial ID ")]
        public string Email_id { get; set; }
        [Required(ErrorMessage = "Please select DOB")]
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Please enter your Address ")]
        public string Address_Communication { get; set; }
        [Required(ErrorMessage = "Please enter your Password ")]
        public string Usr_pwd { get; set; }
        [Required(ErrorMessage = "Please select your Gender ")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Please select your User type ")]
        public string user_type { get; set; }
      
        public int Uidno { get; set; }
    }
    public class deleteUserModel
    {
        public int uidNo { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
       

    }

}