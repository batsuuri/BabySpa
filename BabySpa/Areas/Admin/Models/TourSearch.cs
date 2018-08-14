using BabySpa.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BabySpa.Areas.Admin.Models
{
    public class TourSearch

    {
        public string TourID { get; set; }
        public int TourSeason { get; set; }
        public string TourName { get; set; }
        public DateTime? SStartDate { get; set; }
        public DateTime? EStartDate { get; set; }
        public DateTime? SEndDate { get; set; }
        public DateTime? EEndDate { get; set; }
        public int[] TourType { get; set; }
        public int[] Status { get; set; }

        public List<Tour> List { get; set; }
    }

}