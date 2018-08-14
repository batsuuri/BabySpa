using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class GalleryViewModel
    {
        public Gallery CurrentGallery{ get; set; }
        public int GalleryID { get; set; }
        public List<Gallery> List { get; set; }
        public string DisplayMode { get; set; }
    }
    public class Gallery
    {
        public int GalleryID { get; set; }
        [Required]
        public string GalleryName { get; set; }
        [Required]
        public DateTime? GalleryDate { get; set; }
        public int? GalleryType { get; set; }
        public string Author { get; set; }
        public string[] Tags { get; set; }
        [Required]
        public int Category { get; set; }
        [Required]
        [AllowHtml]
        public string GalleryDesc { get; set; }
        public int IsFeatured { get; set; }
        public string TOURID { get; set; }
        public string CoverUrl { get; set; }
        public GalleryPhotoViewModel GalleryPhotoViewModel { get; set; }
    }

    public class GalleryPhoto
    { 
        public int PhotoID { get; set; }
        public int GalleryID { get; set; }
        public string PhotoName { get; set; }
        public string PhotoUrl { get; set; }
        public string Author { get; set; }
        public string DateTaken { get; set; }
        public string PhotoDesc { get; set; }
        public int IsVideo { get; set; }
        public string[] Tags { get; set; }

    }

    public class GalleryPhotoViewModel
    {
        public GalleryPhoto CurrentPhoto { get; set; }
        public int PhotoID { get; set; }
        public int GalleryID { get; set; }
        public List<GalleryPhoto> List { get; set; }
        public string DisplayMode { get; set; }
    }
}