using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class Staff
    {

        public int StaffID { get; set; }
        [Required]
        public string StaffFName { get; set; }
        [Required]
        public string StaffLName { get; set; }
        [Required]
        [AllowHtml]
        public string StaffDesc { get; set; }
        public string EMail{ get; set; }
        [Required]
        public string Phone { get; set; }
        public string Position { get; set; }
        public int isActive{ get; set; }
        public string[] PhotoUrl { get; set; }
    }

    public class StaffViewModel
    {
        public Staff CurrentStaff{ get; set; }
        public int StaffID { get; set; }
        public List<Staff> List {get;set;}
        public string DisplayMode { get; set; }
    }
}