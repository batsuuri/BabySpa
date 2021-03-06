﻿using System.Web;
using System.Web.Optimization;

namespace BabySpa
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/mdb").Include(
                      "~/Scripts/mdb.min.js",
                      "~/Scripts/popper.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/vendors").Include(
             "~/Scripts/moment.js"
            , "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-theme.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/mdb.min.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/vendors").Include(
                       "~/Content/bootstrap-datetimepicker.css"));

        }
    }
}
