using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
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
        
        [HttpPost]
        public ActionResult RefreshBasic()
        {
            try
            {
                AppConfig.InitDic();
                return Content("Successfully refreshed Basic data.");
            }
            catch (Exception ex)
            {
                Main.ErrorLog("RefreshBasic From Admin", ex);
                return Content("Error:" + ex.Message);
            }

        }
    }
    
}