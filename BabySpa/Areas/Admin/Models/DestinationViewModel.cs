using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Destination
    {

        public int DestinationID { get; set; }
        [Required]
        public string DestinationName { get; set; }
        [Required]
        [AllowHtml]
        public string DestinationDesc { get; set; }
        public string GeoLocation { get; set; }
        public string Province { get; set; }
        public string Soum { get; set; }
        public string[] PhotoUrl { get; set; }
    }

    public class DestinationViewModel
    {
        public Destination CurrentDestination{ get; set; }
        public int DestinationID { get; set; }
        public List<Destination> List {get;set;}
        public string DisplayMode { get; set; }
    }
}