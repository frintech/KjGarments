//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KJGarmentsWeb.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Brand
    {
        public int BRND_id { get; set; }
        public string BRND_Name { get; set; }
        public string BRND_Description { get; set; }
        public string BRND_Img_Path { get; set; }
        public bool Is_Add_img { get; set; }
        public int Add_User_id { get; set; }
        public System.DateTime Add_Datetime { get; set; }
        public int Mod_User_Id { get; set; }
        public System.DateTime Modify_Datetime { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> cat_id { get; set; }
        public Nullable<int> sub_cat_id { get; set; }
    }
}