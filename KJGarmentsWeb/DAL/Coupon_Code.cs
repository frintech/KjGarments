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
    
    public partial class Coupon_Code
    {
        public int Cupn_Id { get; set; }
        public string Cupn_Code { get; set; }
        public string Cupn_Name { get; set; }
        public string Description { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int MinQty { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public Nullable<bool> Active { get; set; }
        public int user_id { get; set; }
        public int Adduser { get; set; }
        public System.DateTime AddDatetime { get; set; }
        public int ModifiedUser { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
