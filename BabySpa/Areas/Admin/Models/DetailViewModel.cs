using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class TourDetailViewModel
    {
        public string TourID { get; set; }
        [AllowHtml]
        public string MapPath { get; set; }
        [AllowHtml]
        public string Included { get; set; }
        [AllowHtml]
        public string NotIncluded { get; set; }
        [AllowHtml]
        public string Package { get; set; }
        [AllowHtml]
        public string Accommodation { get; set; }
        [AllowHtml]
        public string RangedPrice{ get; set; }
        public string GoogleMapUrl { get; set; }

    }
}