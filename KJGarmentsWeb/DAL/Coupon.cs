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
    
    public partial class Coupon
    {
        public int cupn_id { get; set; }
        public string cupn_Code { get; set; }
        public string cupn_Description { get; set; }
        public long cupn_value { get; set; }
        public int Add_User_id { get; set; }
        public System.DateTime Add_Datetime { get; set; }
        public int Mod_User_Id { get; set; }
        public System.DateTime Modify_Datetime { get; set; }
        public Nullable<int> OFR_Value_type { get; set; }
    }
}
