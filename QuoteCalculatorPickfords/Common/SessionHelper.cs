using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Common
{
    public class SessionHelper
    {
        public static int COMPANY_ID = 2;

        public static int InternationalRemovalId
        {
            get
            {
                return HttpContext.Current.Session["InternationalRemovalId"] == null ? 0 : (int)HttpContext.Current.Session["InternationalRemovalId"];
            }
            set { HttpContext.Current.Session["InternationalRemovalId"] = value; }
        }
        public static int QuoteId
        {
            get
            {
                return HttpContext.Current.Session["QuoteId"] == null ? 0 : (int)HttpContext.Current.Session["QuoteId"];
            }
            set { HttpContext.Current.Session["QuoteId"] = value; }
        }

        public static string ToCountryCode
        {
            get
            {
                return HttpContext.Current.Session["ToCountryCode"] == null ? "" : (string)HttpContext.Current.Session["ToCountryCode"];
            }
            set
            {
                HttpContext.Current.Session["ToCountryCode"] = value;
            }
        }

        public static string QuoteType
        {
            get
            {
                return HttpContext.Current.Session["QuoteType"] == null ? "" : (string)HttpContext.Current.Session["QuoteType"];
            }
            set
            {
                HttpContext.Current.Session["QuoteType"] = value;
            }
        }
    }
}