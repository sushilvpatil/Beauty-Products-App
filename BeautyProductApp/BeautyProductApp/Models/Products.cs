using System;
using System.Collections.Generic;
using System.Text;

namespace BeautyProductApp.Models
{
    public class Products
    {
        public string uuid { get; set; }
        public string product_name { get; set; }
        public string brand_name { get; set; }
        public string description { get; set; }
        public string image_url_small { get; set; }
        public string image_url_large { get; set; }
        public string product_category { get; set; }
        public int hazard_rating { get; set; }
        public string hazard_rating_string { get; set; }
        public string hazard_rating_category { get; set; }

    }

    public class ResponseModel
    {
        public List<Products> products { get; set; }
    }

}
