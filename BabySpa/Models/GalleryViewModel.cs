using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BabySpa.Areas.Admin.Models;
namespace BabySpa.Models
{
    public class GalleryViewModel : BaseModel
    {
        public Gallery CurrentGallery { get; set; }
        public int GalleryID { get; set; }
        public List<Gallery> List { get; set; }
        public List<GalleryTags> TagList { get; set; }
        public List<GalleryCategory> GategoryList { get; set; }
        public GalleryFilter Filter { get; set; }
        public string DisplayMode { get; set; }
        public bool HaveNext { get; set; }
    }
    public class GalleryTags
    {
        public int Count { get; set; }
        public string TagName { get; set; }
        public int TagID { get; set; }
    }
    public class GalleryCategory
    {
        public int Count { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
    }
    public class GalleryFilter
    {
        public int GalleryID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string GalleryType { get; set; }
        public string[] Tags { get; set; }
        public string GalleryDate { get; set; }
        public string Author { get; set; }
        public int Page { get; set; }
    }
}