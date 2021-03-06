using System.Web;
using System.Web.Optimization;

namespace WHMIS_Project
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                          "~/Scripts/jquery-ui.js",
                              "~/Scripts/jquery-3.3.1.min.js",
                            "~/Scripts/moment.min.js",                           
                              "~/Scripts/jquery-ui-1.12.1.js",
                                "~/Scripts/jquery-ui-1.12.1.min.js",
                                  "~/Scripts/jquery.validate.min.js",
                                    "~/Scripts/jquery.validate.unobtrusive.min.js",
                         "~/Scripts/DataTables/jquery.dataTables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/DataTables/dataTables.bootstrap4.min.js",                
                    "~/Scripts/bootstrap.min.js",                  
                "~/Scripts/bootstrap.js"));
            
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                   
                      "~/Content/DataTables/css/dataTables.bootstrap.min.css",
                        "~/Content/DataTables/css/responsive.bootstrap.min.css",                  
                       "~/Content/jquery-ui.css",
                        "~/Content/jquery-ui.theme.css",
                         "~/Content/themes/base/jquery-ui.css",
                      "~/Content/site.css"));
         
        }
    }
}
