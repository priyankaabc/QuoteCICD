using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Common
{
    public class SessionHelper
    {
        public static int _COMPANY_ID = 0;

        public static int COMPANY_ID
        {
            get
            {
                if (_COMPANY_ID == 0)
                    _COMPANY_ID = (HttpContext.Current == null || HttpContext.Current.Session["COMPANY_ID"] == null) ? 0 : (int)HttpContext.Current.Session["COMPANY_ID"];
                return _COMPANY_ID;
            }
            set
            {
                HttpContext.Current.Session["COMPANY_ID"] = value;
                _COMPANY_ID = value;
            }
        }

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

        public static string FromCountryCode
        {
            get
            {
                return HttpContext.Current.Session["FromCountryCode"] == null ? "" : (string)HttpContext.Current.Session["FromCountryCode"];
            }
            set
            {
                HttpContext.Current.Session["FromCountryCode"] = value;
            }
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
        public static int VehicleId
        {
            get
            {
                return HttpContext.Current.Session["VehicleId"] == null ? 0 : (int)HttpContext.Current.Session["VehicleId"];
            }
            set { HttpContext.Current.Session["VehicleId"] = value; }
        }
        public static int BaggageId
        {
            get
            {
                return HttpContext.Current.Session["BaggageId"] == null ? 0 : (int)HttpContext.Current.Session["BaggageId"];
            }
            set { HttpContext.Current.Session["BaggageId"] = value; }
        }
        public static bool isOldQuote
        {
            get
            {
                return HttpContext.Current.Session["isOldQuote"] == null ? false : (bool)HttpContext.Current.Session["isOldQuote"];
            }            
            set { HttpContext.Current.Session["isOldQuote"] = value; }
        }
        public static int newBaggageId
        {
            get
            {
                return HttpContext.Current.Session["newBaggageId"] == null ? 0 : (int)HttpContext.Current.Session["newBaggageId"];
            }
            set { HttpContext.Current.Session["newBaggageId"] = value; }
        }

        public static bool TempSessionForMoveDetails
        {
            get
            {
                return HttpContext.Current.Session["TempSessionForMoveDetails"] == null ? false : (bool)HttpContext.Current.Session["TempSessionForMoveDetails"];
            }
            set { HttpContext.Current.Session["TempSessionForMoveDetails"] = value; }
        }
    }
}