using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BabySpa.Areas.Admin.Models;
namespace BabySpa.Models
{
    public class BlogViewModel : BaseModel
    {
        public Blog CurrentBlog { get; set; }
        public int BlogID { get; set; }
        public List<Blog> List { get; set; }
        public List<BlogTags> TagList { get; set; }
        public List<BlogCalendar> CalendarList { get; set; }
        public BlogFilter Filter { get; set; }
        public string DisplayMode { get; set; }
        public bool HaveNext { get; set; }
    }
    public class BlogTags
    {
        public int Count { get; set; }
        public string TagName { get; set; }
        public int TagID { get; set; }
    }
    public class BlogCalendar
    {
        public int Count { get; set; }
        public string DisplayText { get; set; }
        public string DateValue { get; set; }
    }
    public class BlogFilter
    {
        public int BlogID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string BlogType { get; set; }
        public string[] Tags { get; set; }
        public string BlogDate { get; set; }
        public string Author { get; set; }
        public int Page { get; set; }
    }
}