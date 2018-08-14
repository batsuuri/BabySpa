using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
namespace BabySpa
{
    public class CustomErrorHandler : HandleErrorAttribute
    {
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
      
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }
          
            //if (filterContext.HttpContext.Session != null) filterContext.HttpContext.Session.RemoveAll();
            // set this to true so that IIS 7 does not use its own error pages
            //filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            Exception ex = filterContext.Exception;

            if (new HttpException(null, ex).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(ex))
            {
                return;
            }

            var currentController = (string)filterContext.RouteData.Values["controller"];
            var currentActionName = (string)filterContext.RouteData.Values["action"];

            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, currentController, currentActionName);
            // if the request is AJAX return JSON else view.
            if (IsAjax(filterContext))
            {
                //Because its a exception raised after ajax invocation
                //Lets return Json
                filterContext.Result = new JsonResult()
                {
                    Data = filterContext.Exception.Message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                // base.OnException(filterContext);
                filterContext.Result = new ViewResult()
                {
                    ViewName = "Error",
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            //filterContext.HttpContext.Response.StatusCode = 500;
            //if want to get different of the request
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            Main.ErrorLog(currentController + "/" + currentActionName, ex);
        }
    }
}