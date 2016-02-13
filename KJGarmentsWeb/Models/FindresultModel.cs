using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    public class FindresultModel
    {
        public int Pid { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public decimal Product_Price { get; set; }
        public string Product_Description { get; set; }
        public int Category_id { get; set; }
        public int Sub_Category_id { get; set; }
        public int offer_id { get; set; }
        public int brand_id { get; set; }
        public string Imagepath { get; set; }
        public string Sub_cat_name { get; set; }
        public string Cat_name { get; set; }
        public string brand_name { get; set; }
    }
}