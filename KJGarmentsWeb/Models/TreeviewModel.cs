using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MughapuWeb.DAL;
namespace MughapuWeb.Models
{
    public class TreeviewModel
    {

            public Nullable<int> menu_id { get; set; }
            public Nullable<int> Parent_Menuid { get; set; }
            public string origin { get; set; }
            public string Item_Name { get; set; }
            public string parent_name { get; set; }
            public IList<TreeviewList> tvlists { set; get; }
            public TreeviewModel()
            {
                tvlists = new List<TreeviewList>();
            }
        
    }
}