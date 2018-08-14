using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BabySpa.Models;
namespace BabySpa.Areas.Admin.Models
{
    
    public class ItineraryViewModel
    {
        public Itinerary CurrentItem { get; set; }
        public string TourID { get; set; }
        public int ItineraryID { get; set; }
        public List<Itinerary> List { get; set; }
    }
}