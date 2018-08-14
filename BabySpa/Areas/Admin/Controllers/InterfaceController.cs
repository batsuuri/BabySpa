using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BabySpa.Areas.Admin.Controllers
{
    [AreaAuthorize("Admin")]
    public class InterfaceController : Controller
    {
        //
        // GET: /Interface/
        public ActionResult Index()
        {
            return View();
        }
	}
}