using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Order(OrderViewModel model)
        {
            return View();
        }
        public JsonResult GetCust(string name)
        {
            return "";
        }
        public JsonResult GetChild(string name)
        {
            return "";
        }
    }
}