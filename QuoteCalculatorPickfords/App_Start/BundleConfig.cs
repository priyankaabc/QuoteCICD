using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace QuoteCalculatorPickfords
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Css
            bundles.Add(new StyleBundle("~/Content/ui").Include("~/Content/css/bootstrap.min.css",
                "~/Content/css/boxicons.min.css",
                "~/Content/css/owl.carousel.min.css",
                "~/Content/css/nice-select.css",
                "~/Content/css/custom.min.css",
                "~/Content/bootstrap-datepicker.css",
                "~/Content/bootstrap-datetimepicker.min.css",
                "~/Content/css/pnotify.custom.min.css"
                ));
            #endregion

            #region Scripts
            var bundle = new ScriptBundle("~/Scripts/HeaderJQuery")
               .Include("~/Scripts/jquery-3.3.1.min.js",
                        //"~/Scripts/jquery-3.3.1.slim.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/Common.js",
                        "~/Scripts/pnotify.custom.min.js"
                );
            bundle.Orderer = new AsIsBundleOrderer();
            bundles.Add(bundle);
            var bundleF = new ScriptBundle("~/Scripts/FooterJQuery")
                .Include(
                    "~/Scripts/bootstrap.bundle.min.js",
                    "~/Scripts/owl.carousel.min.js",
                    "~/Scripts/jquery.nice-select.min.js",
                    "~/Scripts/bootstrap-datepicker.min.js",
                    "~/Scripts/moment.min.js",
                    "~/Scripts/bootstrap-datetimepicker.min.js",
                    "~/Scripts/bootbox.js"
                 );
            bundleF.Orderer = new AsIsBundleOrderer();
            bundles.Add(bundleF);
            #endregion
        }
        public class AsIsBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }
    }
}
