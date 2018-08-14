using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Accommodation
    {

        public int AccommodationID { get; set; }
        [Required]
        public string AccommodationName { get; set; }
        [Required]
        [AllowHtml]
        public string AccommodationDesc { get; set; }
        //public int AccommodationType { get; set; }
        //public int AccommodationClass { get; set; }
        //public int DestinationID { get; set; }
        public string[] PhotoUrl { get; set; }
    }

    public class AccommodationViewModel
    {
        public Accommodation CurrentAccommodation{ get; set; }
        public int AccommodationID { get; set; }
        public List<Accommodation> List {get;set;}
        public string DisplayMode { get; set; }
    }
}