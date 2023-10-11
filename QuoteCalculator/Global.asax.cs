using NLog;
using QuoteCalculator.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QuoteCalculator
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IN");
            CultureInfo info = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
            info.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = info;
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            if (url.Contains("http://") && !url.ToLower().Contains("localhost"))
            {
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
            }
        }

        private void SetCompanyId()
        {
            try
            {
                string siteName = System.Web.Hosting.HostingEnvironment.SiteName.ToUpper();
                if (siteName.Contains("ANGLO") || siteName.Contains("QUOTECALCULATOR"))
                    SessionHelper.COMPANY_ID = 1;
                else if (siteName.Contains("PICKFORD"))
                    SessionHelper.COMPANY_ID = 2;
                else
                {
                    if (siteName.Contains("EXCESS"))
                        SessionHelper.COMPANY_ID = 3;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
