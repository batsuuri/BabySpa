using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BabySpa.Core;
using static BabySpa.App;
using System.Web.Mvc;

namespace BabySpa.Models
{
    [Serializable]
    public class Customer:BaseModel
    {
        public Customer()
        {
            PageInfo = new PageInfo();
        }
        [Key]
        public int CustID { get; set; }
        public int GroupID { get; set; }

        public DateTime RegDate { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string SocialID { get; set; }
        public string Title { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Languages { get; set; }
        public string PassportNo { get; set; }
        public string PassportValidDate { get; set; }
        public int MembershipID { get; set; }

        public int IsGroupLeader { get; set; }
        public int Status { get; set; }
        public string HearUS { get; set; }
        public int UBFlightRequired { get; set; }
        public int ExtraRoomRequired { get; set; }
        public int JoinGroup { get; set; }
        public DateTime? ArriveDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string MedicalCondition { get; set; }
        public string MealRequirement { get; set; }
        public string Comments { get; set; }
        public string WhyChooseReason { get; set; }
        [AllowHtml]
        public string AdminNote { get; set; }
        public Result _result { get; set; }
        
    }

}