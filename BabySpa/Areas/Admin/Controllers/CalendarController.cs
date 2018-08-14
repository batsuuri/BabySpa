using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class CalendarController : Controller
    {
        //
        // GET: /Calendar/
        public ActionResult Index()
        {
            return View();
        }
	}
}