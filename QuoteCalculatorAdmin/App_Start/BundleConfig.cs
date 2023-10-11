using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace QuoteCalculatorAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862 
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region Css

            bundles.Add(new StyleBundle("~/Content/ui").Include(
                "~/Content/css/icomoon.css",
                "~/Content/css/bootstrap.css",
                "~/Content/css/core.css",
                "~/Content/css/components.css",
                "~/Content/css/colors.css",
                "~/Content/css/bootbox.css",
                "~/Content/css/pnotify.custom.min.css",
                "~/Content/css/boxicons.min.css"
                ));

            bundles.Add(new StyleBundle("~/content/NewUI_Style").Include(
                    "~/Content/bootstrap4.5.1/css/bootstrap.min.css",
                    "~/Content/bootstrap-select/css/bootstrap-select.css",
                    "~/Content/css/custom.css",
                    "~/Content/css/media.css",
                    "~/Content/select2/css/select2.min.css",
                    "~/Content/css/icomoon.css"                    
                ));
                //bundles.Add(new StyleBundle("~/Content/AngloCSS").Include(
                // "~Content/new-theme/css/custom.css"
                // ));

                //bundles.Add(new StyleBundle("~/Content/PickfordsCSS").Include(
                //  "~/Content/css/pickfordscustom.min.css"
                //  ));

                //bundles.Add(new StyleBundle("~/Content/ExcessBaggageCSS").Include(
                // "~/Content/css/new-theme/css/excess-custom.css"
                // ));
            #endregion

            #region Scripts

            var bundleJquery = new ScriptBundle("~/Scripts/JQueryScripts").Include(
                     "~/Scripts/jquery.validate.min.js",
                     "~/Scripts/jquery.validate.unobtrusive.min.js"
                      );
            bundleJquery.Orderer = new AsIsBundleOrderer();
            bundles.Add(bundleJquery);



            bundles.Add(new ScriptBundle("~/Scripts/LoginJQuery").Include("~/Scripts/bootstrap.min.js",
                    "~/Scripts/blockui.min.js",
                    "~/Scripts/nicescroll.min.js",
                    "~/Scripts/drilldown.js",
                    //"~/Scripts/app.js",
                    "~/Scripts/uniform.min.js"
                   ));


            var bundle = new ScriptBundle("~/Scripts/NewUI_Scripts").Include(
                ////"~/Content/popper.js/umd/popper.js",
                ////"~/Content/bootstrap4.5.1/js/bootstrap.min.js",
                //"~/Content/bootstrap4.5.1/js/bootstrap.bundle.min.js",
                "~/Content/new-theme/vendor/bootstrap/js/bootstrap.bundle.min.js",
                ////"~/Content/popper.js/umd/popper.js",
                "~/Content/select2/js/select2.min.js",
                "~/Content/bootstrap-select/js/bootstrap-select.min.js"
                );

            bundle.Orderer = new AsIsBundleOrderer();


            bundles.Add(bundle);


            #endregion
        }

        public class AsIsBundleOrderer : IBundleOrderer
        {
            //var aa = 0; 
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {    
                return files;
            }

        }
    }
}





////bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
////            "~/Scripts/jquery-{version}.js"));

////bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
////            "~/Scripts/jquery.validate*"));

////// Use the development version of Modernizr to develop with and learn from. Then, when you're
////// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
////bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
////            "~/Scripts/modernizr-*"));

////bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
////          "~/Scripts/bootstrap.js"));

////bundles.Add(new StyleBundle("~/Content/css").Include(
////          "~/Content/bootstrap.css",
////          "~/Content/site.css"));

//#region Css

//bundles.Add(new StyleBundle("~/content/NewUI_Style").Include(
//        "~/Content/bootstrap4.5.1/css/bootstrap.min.css",
//        "~/Content/css/custom.css",
//        //"~/Content/css/bootstrap.css",
//        "~/Content/select2/css/select2.min.css",
//        "~/Content/css/icomoon.css"
//    //"~/Content/css/components.css",
//    //"~/Content/css/colors.css",
//    //"~/Content/css/bootbox.css",
//    //"~/Content/css/pnotify.custom.min.css",
//    //"~/Content/css/boxicons.min.css",


//    ));



//bundles.Add(new StyleBundle("~/Content/ui").Include(
//    "~/Content/css/icomoon.css",
//    "~/Content/css/bootstrap.css",
//    //"~/Content/css/core.css",
//    "~/Content/css/components.css",
//    "~/Content/css/colors.css",
//    "~/Content/css/bootbox.css",
//    "~/Content/css/pnotify.custom.min.css",
//    "~/Content/css/boxicons.min.css"
//    ));



//bundles.Add(new StyleBundle("~/Content/kendo").Include(
//    "~/Content/kendo-ui/styles/kendo.common.min.css",
//    "~/Content/kendo-ui/styles/kendo.silver.min.css",
//    "~/Content/kendo-ui/styles/kendo.custom.css",
//    //"~/Content/kendo-ui/styles/kendoCustom.css",
//    "~/Content/css/pnotify.custom.min.css"
//   ));

//bundles.Add(new StyleBundle("~/Content/kendo_pf").Include(
//   "~/Content/kendo-ui/styles/kendo.common.min.css",
//   "~/Content/kendo-ui/styles/kendo.silver.min.css",
//   "~/Content/kendo-ui/styles/pf_kendo.custom.css",
//   "~/Content/kendo-ui/styles/kendoCustom.css",
//   "~/Content/css/pnotify.custom.min.css"
//  ));

//bundles.Add(new StyleBundle("~/Content/kendo_ei").Include(
//   "~/Content/kendo-ui/styles/kendo.common.min.css",
//   "~/Content/kendo-ui/styles/kendo.silver.min.css",
//   "~/Content/kendo-ui/styles/ei_kendo.custom.css",
//   "~/Content/kendo-ui/styles/kendoCustom.css",
//   "~/Content/css/pnotify.custom.min.css"
//  ));
//#endregion

//#region Scripts



//bundles.Add(new ScriptBundle("~/Scripts/HeaderJQuery").Include(
//    "~/Scripts/jquery.min.js",
//    "~/Content/kendo-ui/js/kendo.all.min.js",
//    "~/Content/kendo-ui/js/kendo.aspnetmvc.min.js"
//   ));

//bundles.Add(new ScriptBundle("~/Scripts/HeaderNewJQuery").Include(
//   //"~/Content/jquery/jquery.min.js"
//   ));

//var bundleJquery = new ScriptBundle("~/Scripts/JQueryScripts").Include(
//         "~/Scripts/jquery.validate.min.js",
//         "~/Scripts/jquery.validate.unobtrusive.min.js"
//          );
//bundleJquery.Orderer = new AsIsBundleOrderer();
//bundles.Add(bundleJquery);



//bundles.Add(new ScriptBundle("~/Scripts/LoginJQuery").Include("~/Scripts/bootstrap.min.js",
//        "~/Scripts/blockui.min.js",
//        "~/Scripts/nicescroll.min.js",
//        "~/Scripts/drilldown.js",
//        "~/Scripts/app.js",
//        "~/Scripts/uniform.min.js"
//       ));


//var bundle = new ScriptBundle("~/Scripts/UIScripts").Include("~/Scripts/bootstrap.min.js",
//        "~/Scripts/blockui.min.js",
//        "~/Scripts/nicescroll.min.js",
//        "~/Scripts/drilldown.js",
//        "~/Scripts/app.js",
//        "~/Scripts/bootbox.js",
//        "~/Content/kendo-ui/js/cultures/kendo.culture.en-IN.min.js",
//        "~/Scripts/Common.js",
//           "~/Scripts/uniform.min.js",
//            //"~/Scripts/validate.min.js",
//            "~/Scripts/cookie.js",
//            "~/Scripts/app.js",
//            "~/Scripts/wizard_steps.js",
//            "~/Scripts/ripple.min.js",
//            "~/Scripts/pnotify.custom.min.js"
//        //"~/Content/datatable/datatable.min.js"
//        );

//bundles.Add(new ScriptBundle("~/Scripts/NewUI_Scripts").Include(
//    "~/Content/bootstrap4.5.1/js/bootstrap.min.js",
//    "~/Content/select2/js/select2.min.js",
//    "~/Scripts/blockui.min.js",
//    "~/Scripts/nicescroll.min.js",
//    "~/Scripts/drilldown.js",
//    "~/Scripts/app.js",
//    "~/Scripts/bootbox.js",
//    "~/Scripts/Common.js",
//    "~/Scripts/uniform.min.js",
//    "~/Scripts/ripple.min.js",
//    "~/Scripts/pnotify.custom.min.js"

//    ));