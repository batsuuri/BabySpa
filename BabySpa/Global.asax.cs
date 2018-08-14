using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            Main.apppath = Server.MapPath("");
            AppConfig.Init();
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session != null)
            {
                CultureInfo ci = (CultureInfo)this.Session["Culture"];
                if (ci == null)
                {
                    string langName = "en";
                    if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length != 0)
                    {
                        langName = HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);
                    }
                    ci = new CultureInfo(langName);
                    this.Session["Culture"] = ci;
                }

                HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
                RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);
                routeData.Values["culture"] = ci;

                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
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
