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
    
    public partial class TreeviewList
    {
        public int id { get; set; }
        public Nullable<int> menu_id { get; set; }
        public Nullable<int> Parent_Menuid { get; set; }
        public string origin { get; set; }
        public string Item_Name { get; set; }
        public string parent_name { get; set; }
    }
}