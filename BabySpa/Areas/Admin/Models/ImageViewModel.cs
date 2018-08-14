using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabySpa.Areas.Admin.Models
{

    public class ImageViewModel
    {
        public Image CurrentImage { get; set; }
        public string ImageName { get; set; }
        public List<Image> List { get; set; }
        public string DisplayMode { get; set; }
    }
    public class Image
    {
        public string ImageExt { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }
}