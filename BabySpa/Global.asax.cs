using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BabySpa
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            Main.apppath = Server.MapPath("");
            AppConfig.Init();
        }
        void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception exception = Server.GetLastError();
                if (exception is HttpUnhandledException)
                {
                    exception = exception.InnerException;
                }
                if (exception != null)
                {
                    Main.ErrorLog(Request.QueryString.ToString(), exception);
                }
                Server.ClearError();
                //Response.Redirect("~/Error/Index");
            }
            catch (Exception ex)
            {
                //Main.ErrorLog("Handling application error ", ex);
            }
        }
    }
}
