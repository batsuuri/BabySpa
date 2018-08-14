using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Transport
    {

        public int TransportID { get; set; }
        [Required]
        public string TransportName { get; set; }
        [Required]
        [AllowHtml]
        public string TransportDesc { get; set; }
        public string Brand { get; set; }
        public string Mark { get; set; }
        public int Type { get; set; }
        public string[] PhotoUrl { get; set; }
    }

    public class TransportViewModel
    {
        public Transport CurrentTransport{ get; set; }
        public int TransportID { get; set; }
        public List<Transport> List {get;set;}
        public string DisplayMode { get; set; }
    }
}