using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Info
    {
        public int InfoOrder { get; set; }
        public string TourID { get; set; }
        public string InfoType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        [AllowHtml]
        public string InfoValue { get; set; }
        [AllowHtml]
        public string InfoValue2 { get; set; }
        public string InfoDesc { get; set; }
        public string InfoKey { get; set; }
    }

    public class InfoViewModel
    {
        public Info CurrentInfo { get; set; }
        public string InfoKey { get; set; }
        public string TourID{ get; set; }
        public List<Info> List {get;set;}
        public string InfoType { get; set; }
        public string DisplayMode { get; set; }
    }
}