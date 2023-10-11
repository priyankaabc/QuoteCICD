using QuoteCalculatorAdmin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Common
{
    public class SessionHelper
    {
        public static int UserId
        {
            get
            {
                return HttpContext.Current.Session["UserId"] == null ? 0 : (int)HttpContext.Current.Session["UserId"];
            }
            set { HttpContext.Current.Session["UserId"] = value; }
        }
        public static int RoleId
        {
            get
            {
                return HttpContext.Current.Session["RoleId"] == null ? 0 : (int)HttpContext.Current.Session["RoleId"];
            }
            set { HttpContext.Current.Session["RoleId"] = value; }
        }
        public static int CompanyId
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                    throw new Exception("Cannot get Company Id. Session is null.");

                return HttpContext.Current.Session["CompanyId"] == null ? 0 : (int)HttpContext.Current.Session["CompanyId"];
            }
            set
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                    throw new Exception("Cannot set Company Id. Session is null.");

                HttpContext.Current.Session["CompanyId"] = value;
            }
        }

        public static string WelcomeUser
        {
            get
            {
                return HttpContext.Current.Session["WelcomeUser"] == null
                    ? "Guest"
                    : (string)HttpContext.Current.Session["WelcomeUser"];
            }
            set { HttpContext.Current.Session["WelcomeUser"] = value; }
        }
        public static string SalesRepCode
        {
            get
            {
                return HttpContext.Current.Session["SalesRepCode"] == null
                    ? "Guest"
                    : (string)HttpContext.Current.Session["SalesRepCode"];
            }
            set { HttpContext.Current.Session["SalesRepCode"] = value; }
        }       
        public static string Email
        {
            get { return Convert.ToString(HttpContext.Current.Session["Email"]); }

            set { HttpContext.Current.Session["Email"] = value; }
        }
        public static int BaggageQuoteId
        {
            get
            {
                return HttpContext.Current.Session["BaggageQuoteId"] == null ? 0 : (int)HttpContext.Current.Session["BaggageQuoteId"];
            }
            set { HttpContext.Current.Session["BaggageQuoteId"] = value; }
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
        //public static bool IsSuperAdmin
        //{
        //    get
        //    {
        //        return HttpContext.Current.Session["IsSuperAdmin"] != null &&
        //               (bool)HttpContext.Current.Session["IsSuperAdmin"];
        //    }
        //    set { HttpContext.Current.Session["IsSuperAdmin"] = value; }
        //}

        //public static int RoleId
        //{
        //    get
        //    {
        //        return HttpContext.Current.Session["RoleId"] == null ? 0 : (int)HttpContext.Current.Session["RoleId"];
        //    }
        //    set { HttpContext.Current.Session["RoleId"] = value; }
        //}

        //public static List<sp_UserAccessPermissions_Result> UserAccessPermissions
        //{
        //    get
        //    {
        //        return HttpContext.Current.Session["UserAccessPermissions"] == null
        //            ? new List<sp_UserAccessPermissions_Result>()
        //            : HttpContext.Current.Session["UserAccessPermissions"] as
        //                List<sp_UserAccessPermissions_Result>;
        //    }

        //    set { HttpContext.Current.Session["UserAccessPermissions"] = value; }
        //}

        public static void RememberLoginDetails(string Email, string Password)
        {
            HttpCookie objCookie = HttpContext.Current.Request.Cookies["EmployeeLoginDetails"] ?? new HttpCookie("EmployeeLoginDetails");
            objCookie.Values["LastVisit"] = DateTime.Now.ToString();
            objCookie.Values["Email"] = Email;
            objCookie.Values["Password"] = Password;
            objCookie.Expires = DateTime.Now.AddDays(30);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }

        public static HttpCookie GetLoginDetails()
        {
            HttpCookie objCookie = HttpContext.Current.Request.Cookies["EmployeeLoginDetails"];
            if (objCookie != null)
            {
                return objCookie;
            }
            return null;
        }

        public static void ClearCookie(string Key)
        {
            HttpCookie objCookie = HttpContext.Current.Request.Cookies[Key] ?? new HttpCookie(Key);
            objCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(objCookie);
        }
        public static List<sp_UserAccessPermissions_Result> UserAccessPermissions
        {
            get
            {
                return HttpContext.Current.Session["UserAccessPermissions"] == null
                    ? new List<sp_UserAccessPermissions_Result>()
                    : HttpContext.Current.Session["UserAccessPermissions"] as
                        List<sp_UserAccessPermissions_Result>;
            }

            set { HttpContext.Current.Session["UserAccessPermissions"] = value; }
        }

    }
}