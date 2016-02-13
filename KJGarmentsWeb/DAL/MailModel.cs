using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace KJGarmentsWeb.DAL
{
    public class MailModel
    {
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
         [Required]
        public string Address { get; set; }
         [Required]
        [EmailAddress]
        public string Email { get; set; }
         [Required]
       //[RegularExpression(@"^[0-9]{0,15}$", ErrorMessage = "Mobile No should contain only numbers")]
         [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered mobile no is not valid.")]  
        public string Mobile { get; set; }
        public string Comments { get; set; }
     

    }

}