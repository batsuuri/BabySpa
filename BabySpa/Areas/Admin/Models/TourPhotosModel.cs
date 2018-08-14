using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class TourPhoto
    {
        public int PhotoID { get; set; }
        public string TourID { get; set; }
        [Required]
        public string PhotoName { get; set; }
        public string Alt { get; set; }
        public string PhotoUrl { get; set; }
        public string DayNo { get; set; }
        public string Author{ get; set; }
        public string DateTaken { get; set; }
        public string PhotoDesc { get; set; }
        public string Place { get; set; }
    }

    public class TourPhotosModel
    {
        public TourPhoto CurrentPhoto{ get; set; }
        public int PhotoID { get; set; }
        public string TourID{ get; set; }
        public List<TourPhoto> List {get;set;}
        public string DisplayMode { get; set; }
        public List<SelectListItem> Days { get; set; }

    }
}