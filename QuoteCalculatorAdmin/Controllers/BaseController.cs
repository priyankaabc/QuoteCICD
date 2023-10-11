using QuoteCalculatorAdmin.Common;
using QuoteCalculatorAdmin.Data;
using QuoteCalculatorAdmin.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace QuoteCalculatorAdmin.Controllers
{

    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = System.Web.HttpContext.Current;

            string controller = (filterContext.Controller.ToString().Split('.')[filterContext.Controller.ToString().Split('.').Length - 1]).ToLower();
            if (controller != "logincontroller" && SessionHelper.UserId == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            status=401,
                            message = "Unauthorized, Access denied due to session time out."
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else 
                {
                    filterContext.Result = new RedirectResult("~/Login/Index");
                }

            }

            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnActionExecuting(filterContext);

             
        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                FormsAuthenticationTicket newTicket = FormsAuthentication.RenewTicketIfOld(ticket);
                if (newTicket != null && newTicket.Expiration != ticket.Expiration)
                {
                    string encryptedTicket = FormsAuthentication.Encrypt(newTicket);

                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        Path = FormsAuthentication.FormsCookiePath
                    };
                    Response.Cookies.Add(cookie);
                }
            }
        }

        private void RedirectToLoginPage(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Login/SessionTimeout");
            return;
        }


        /// <summary>
        /// Set Session Values
        /// </summary>
        /// <param name="filterContext"></param>
        /// 
        private void SetSessionValues(ActionExecutingContext filterContext)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                RedirectToLoginPage(filterContext);
            }
            else
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null || authTicket.Expired)
                {
                    RedirectToLoginPage(filterContext);
                }
                else
                {
                    string userName = authTicket.Name;

                    user logedInUser = new GenericRepository<user>().GetEntities().FirstOrDefault(u => u.username == userName);

                    if (logedInUser == null)
                    {
                        RedirectToLoginPage(filterContext);
                    }
                    //else if (logedInUser.IsActive == false)
                    //{
                    //    RedirectToLoginPage(filterContext);
                    //}
                    else
                    {
                        SessionHelper.UserId = Convert.ToInt32(logedInUser.id);
                        SessionHelper.WelcomeUser = logedInUser.username;
                        SessionHelper.Email = logedInUser.email;
                        // SessionHelper.IsSuperAdmin = logedInUser.IsSuperAdmin;

                    }
                }


            }
        }
        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">The Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void ErrorNotification(string msg)
        {
            TempData[CustomEnums.NotifyType.Error.GetDescription()] = msg;
        }
        /// <summary>
        /// Display Sucess notification
        /// </summary>
        /// <param name="exception">The Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        protected virtual void SuccessNotification(string msg)
        {
            TempData[CustomEnums.NotifyType.Success.GetDescription()] = msg;
        }
    }
    
}