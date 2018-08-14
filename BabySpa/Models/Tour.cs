using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EM.Core;
using static EM.App;
using System.Web.Mvc;

namespace EM.Models
{
    [Serializable]
    public class Tour:BaseModel
    {
        public Tour()
        {
            PageInfo = new PageInfo();
        }

        [Required]
        public string TourId { get; set; }
        
        public string TourName { get; set; }
        public int TourType { get; set; }
        public string TypeName{ get; set; }
        public string TourNameShort { get; set; }
        public int TourSeason{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public int NightCount  { get; set; }
        public int DayCount { get; set; }
        public string Destination { get; set; }
        [AllowHtml]
        public string BasicInfo { get; set; }
        public int MaxGroupSize { get; set; }
        public int MinGroupSize { get; set; }
        public string TourCur { get; set; }
        public decimal TourPrice { get; set; }
        public decimal Deposit { get; set; }
        public decimal Discount { get; set; }
        public decimal TourPriceMNT { get; set; }
        public int CurrentEntrantCount { get; set; }
        public byte ShowInHome { get; set; }
        public string CoverPhoto{ get; set; }
        [AllowHtml]
        public string CoverText { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }

        public string SmallGroupCount { get; set; }
        public decimal SmallGroupSupplement { get; set; }
        public decimal SingleSupplement { get; set; }
        public string CoverTextAlign { get; set; }
        public string CoverDateTextAlign { get; set; }
        public string CoverPhotoAlt{ get; set; }
        public string CoverDateText{ get; set; }
        public byte Category { get; set; }
        public string CategoryName{ get; set; }
        [AllowHtml]
        public string Character { get; set; }
        public string[] RangedPrice { get; set; }
        public string TourMapUrl { get; set; }

        public Result _result { get; set; }
            
        public List<Itinerary> TourItinerary { get; set; }
        public List<InfoData> TourInfoData { get; set; }
        public List<TourPhoto> TourPhoto { get; set; }

        public List<Tour> tourList { get; set; }


        public string GetRes(string columnName, string defaultvalue)
        {
            return GetRes("TOUR", columnName, this.TourId, defaultvalue);
        }
    }

    [Serializable]
    public class Itinerary : BaseModel
    {
        public Itinerary()
            {
            Transport  =new Dictionary();
            Accommodation = new Dictionary(); Accommodation2 = new Dictionary(); Accommodation3 = new Dictionary();
            Destination = new Dictionary(); Destination2 = new Dictionary(); Destination3 = new Dictionary();
            BreakFast = new Dictionary(); BreakFast2 = new Dictionary(); BreakFast3 = new Dictionary();
            Dinner = new Dictionary(); Dinner2 = new Dictionary(); Dinner3 = new Dictionary();
            Lunch = new Dictionary(); Lunch2 = new Dictionary(); Lunch3 = new Dictionary(); 
            }
        public int ItineraryID { get; set; }
        public string TourId { get; set; }
        public string DayNo { get; set; }
        [AllowHtml]
        public string Subject { get; set; }
        [AllowHtml]
        public string Texts { get; set; }
        public string PhotoName { get; set; }
        [AllowHtml]
        public string Hint { get; set; }
        public string Duration { get; set; }
        public string Duration2 { get; set; }
        public string Duration3 { get; set; }
        public string Distance { get; set; }
        public string Distance2 { get; set; }
        public string Distance3 { get; set; }
        public int OrderNo { get; set; }

        public Dictionary Accommodation { get; set; }
        public Dictionary Accommodation2 { get; set; }
        public Dictionary Accommodation3 { get; set; }

        public Dictionary Transport { get; set; }

        public Dictionary Destination { get; set; }
        public Dictionary Destination2 { get; set; }
        public Dictionary Destination3 { get; set; }
      
        public Dictionary Lunch { get; set; }
        public Dictionary Lunch2 { get; set; }
        public Dictionary Lunch3 { get; set; }

        public Dictionary BreakFast { get; set; }
        public Dictionary BreakFast2 { get; set; }
        public Dictionary BreakFast3 { get; set; }
        
        public Dictionary Dinner { get; set; }
        public Dictionary Dinner2 { get; set; }
        public Dictionary Dinner3 { get; set; }
        public List<Itinerary> TourItinerary { get; set; }
    }
    public class InfoData : BaseModel
    {
        public string TourId { get; set; }
        public string InfoType { get; set; }
        public string InfoValue { get; set; }
        public string InfoValue2 { get; set; }
        public string InfoDesc { get; set; }
        public int InfoOrder { get; set; }
        public int InfoKey{ get; set; }
    }
    public class TourPhoto : BaseModel
    {
        public string TourId { get; set; }
        public string PhotoName { get; set; }
        public string Alt { get; set; }
        public string PhotoUrl { get; set; }
        public string DayNo { get; set; }
        public string Author { get; set; }
        public string DateTaken { get; set; }
        public string PhotoDesc { get; set; }
        public string Place { get; set; }
        public int PhotoID { get; set; }
    }
   
    public enum eTourTypes {
        photo = 1,
        fish = 2,
        custom = 3,
        logistic = 4,
        service = 5,
    }

    public enum eTourStatus
    {
        Open = 1,
        Completed = 2,
        Canceled = 3,
        KickOff = 4,
        Planned = 5,
    }

    public enum eTourCategory
    {
        GoldenEagle = 1,
        NaadamPestival = 2,
        MonthlyFish = 3,
        WinterFish = 4,
        Planned = 5
    }
}