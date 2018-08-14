using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Models
{
    public class BlogViewModel
    {
        public Blog CurrentBlog{ get; set; }
        public int BlogID { get; set; }
        public List<Blog> List { get; set; }
        public string DisplayMode { get; set; }
    }
    public class Blog
    {
        public int BlogID { get; set; }
        [Required]
        public string BlogName { get; set; }
        [Required]
        public DateTime? BlogDate { get; set; }
        public int? BlogType { get; set; }
        public string Author { get; set; }
        public string[] Tags { get; set; }
        [Required]
        public int Category { get; set; }
        [Required]
        [AllowHtml]
        public string BlogContent { get; set; }
        public int IsFeatured { get; set; }
        public string TOURID { get; set; }
        public string CoverUrl { get; set; }

    }
}