using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Food
    {

        public int FoodID { get; set; }
        [Required]
        public string FoodName { get; set; }
        [Required]
        [AllowHtml]
        public string FoodDesc { get; set; }
        //public int FoodType { get; set; }
        public string[] PhotoUrl { get; set; }
    }

    public class FoodViewModel
    {
        public Food CurrentFood{ get; set; }
        public int FoodID { get; set; }
        public List<Food> List {get;set;}
        public string DisplayMode { get; set; }
    }
}