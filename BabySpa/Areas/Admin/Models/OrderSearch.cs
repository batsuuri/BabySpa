using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BabySpa.Areas.Admin.Models
{
    public class OrderSearch

    {
        public string TourID { get; set; }
        public int? CustID { get; set; }
        public string CustName { get; set; }
        public string Email { get; set; }
        public string TourName { get; set; }
        public int? OrderID { get; set; }

        public DateTime? SOrderDate { get; set; }
        public DateTime? EOrderDate { get; set; }
       
        public DateTime? SCompletedDate { get; set; }
        public DateTime? ECompletedDate { get; set; }
        public int? StaffID { get; set; }
        public int[] Status { get; set; }
        public int? AgentID { get; set; }
        public int[] TourType { get; set; }
        public int TourSeason{ get; set; }
        public DateTime? STourDate { get; set; }
        public DateTime? ETourDate { get; set; }
        public DataTable List { get; set; }
    }

}