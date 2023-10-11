using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorAdmin.Common
{
    public class AuthorizationHelper
    {
        public static bool CanAdd(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => item.IsInsert == true && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }

        public static bool CanEdit(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => item.IsEdit == true && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }

        public static bool CanDelete(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => item.IsDelete == true && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }

        public static bool CanChangeStatus(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => item.IsChangeStatus == true && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }

        public static bool CanEditDelete(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => (item.IsDelete == true || item.IsEdit == true)

            && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }
        public static bool IsAuthorized(string controllerName)
        {
            return SessionHelper.UserAccessPermissions.Any(item => item.IsView == true && item.Controller != null && item.Controller.ToLower() == controllerName.ToLower());
        }
    }
}