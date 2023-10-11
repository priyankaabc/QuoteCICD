using QuoteCalculatorAdmin.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculatorAdmin.Common
{
    public static class HtmlHelperExtensions
    {
        public static string SetStatusClientTemplate(this HtmlHelper helper, string isActive, string controllerName, string action, string parameter, string id, string gridId, string entityName)
        {

            string deactivteMessage = "Are you sure you want to deactivate this " + entityName + " ?";


            string activteMessage = "Are you sure you want to activate this " + entityName + " ?";

            var deactiveAttributes = " onclick='changeStatus(" + @"""" + controllerName + @"""" + ", " + @"""" +
                                    action + @"""" + ", " + @"""""" + ", "
                                    + @"""" + parameter + @"""" + ", " + @"""" + deactivteMessage + @"""" + ", " + id
                                    + ", " + @"""" + gridId + @"""" + @")'";

            var activeAttributes = " onclick='changeStatus(" + @"""" + controllerName + @"""" + ", " + @"""" +
                                   action + @"""" + ", " + @"""""" + ", "
                                   + @"""" + parameter + @"""" + ", " + @"""" + activteMessage + @"""" + ", " + id
                                   + ", " + @"""" + gridId + @"""" + @")'";


            return "# if (" + isActive + ")    {#" +
                     @"<a class='k-button' " + deactiveAttributes + @"><span class='k-icon k-i-check'></span></a>" +
                     "#}else { #" +
                     @"<a class='k-button' " + activeAttributes + @"><span class='k-icon k-i-close'></span></a>"
                     + "#}#";
        }

        public static string SetAllowPushDataClientTemplate(this HtmlHelper helper, string isallowActive, string controllerName, string action, string parameter, string id, string gridId, string entityName)
        {

            string deactivteMessage = "Are you sure you want not to allow for Push Dataset ";


            string activteMessage = "Are you sure you want to allow for Push Dataset ";

            var deactiveAttributes = " onclick='changeAllowDatasetStatus(" + @"""" + controllerName + @"""" + ", " + @"""" +
                                    action + @"""" + ", " + @"""""" + ", "
                                    + @"""" + parameter + @"""" + ", " + @"""" + deactivteMessage + @"""" + ", " + id
                                    + ", " + @"""" + gridId + @"""" + @")'";

            var activeAttributes = " onclick='changeAllowDatasetStatus(" + @"""" + controllerName + @"""" + ", " + @"""" +
                                   action + @"""" + ", " + @"""""" + ", "
                                   + @"""" + parameter + @"""" + ", " + @"""" + activteMessage + @"""" + ", " + id
                                   + ", " + @"""" + gridId + @"""" + @")'";


            return "# if (" + isallowActive + ")    {#" +
                     @"<a class='k-button' " + deactiveAttributes + @"><span class='k-icon k-i-check'></span></a>" +
                     "#}else { #" +
                     @"<a class='k-button' " + activeAttributes + @"><span class='k-icon k-i-close'></span></a>"
                     + "#}#";
        }

        //public static MvcHtmlString GenerateMenu(this HtmlHelper helper)
        //{
        //    List<sp_UserAccessPermissions_Result> parentMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId == null).OrderBy(item => item.DisplayOrder).ToList();

        //    List<sp_UserAccessPermissions_Result> childMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId != null).OrderBy(item => item.DisplayOrder).ToList();

        //    if (parentMenuList.Any())
        //    {
        //        TagBuilder ul = new TagBuilder("ul");
        //        ul.MergeAttribute("class", "nav navbar-nav navbar-nav-material");
        //        ul.MergeAttribute("id", "sideBar");

        //        StringBuilder sb = new StringBuilder();

        //        foreach (sp_UserAccessPermissions_Result menu in parentMenuList)
        //        {
        //            List<sp_UserAccessPermissions_Result> childList = childMenuList.Where(x => x.ParentMenuId == menu.MenuId).ToList();

        //            if (childList.Any())
        //            {

        //            }
        //            else
        //            {
        //                TagBuilder imageTag = ITag(menu.ImagePath);

        //                TagBuilder aLink = AnchorLink(menu.Action, menu.Controller, false);
        //                aLink.InnerHtml = string.Format("{0}{1}", Convert.ToString(imageTag), menu.Name);
        //                aLink.MergeAttribute("class", "legitRipple");


        //                TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
        //                sb.Append(Convert.ToString(liWithChild));
        //            }
        //        }

        //        ul.InnerHtml = sb.ToString();
        //        return MvcHtmlString.Create(ul.ToString());
        //    }

        //    return MvcHtmlString.Empty;
        //}
        //public static MvcHtmlString GenerateMenu12(this HtmlHelper helper)
        //{
        //    List<sp_UserAccessPermissions_Result> parentMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId == null).OrderBy(item => item.DisplayOrder).ToList();

        //    List<sp_UserAccessPermissions_Result> childMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId != null).OrderBy(item => item.DisplayOrder).ToList();

        //    if (parentMenuList.Any())
        //    {
        //        TagBuilder ul = new TagBuilder("ul");
        //        //ul.MergeAttribute("class", "navigation navigation-main navigation-accordion");
        //        ul.MergeAttribute("class", "navbar-nav mr-auto");
        //        //ul.MergeAttribute("id", "sideBar");

        //        StringBuilder sb = new StringBuilder();

        //        foreach (sp_UserAccessPermissions_Result menu in parentMenuList)
        //        {
        //            List<sp_UserAccessPermissions_Result> childList = childMenuList.Where(x => x.ParentMenuId == menu.MenuId).ToList();

        //            if (childList.Any())
        //            {
        //                StringBuilder sbChild = new StringBuilder();


        //                //li name 
        //                TagBuilder liWithChild = new TagBuilder("li");
        //                liWithChild.AddCssClass("nav-item dropdown");


        //                //Static a tag
        //                TagBuilder firstSpan = ITag("icon-make-group position-left");
        //                TagBuilder secondSpan = SpanTag("caret");

        //                TagBuilder aLink = AnchorLink(null, null, true);
        //                aLink.AddCssClass("nav-link");
        //                aLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Convert.ToString(firstSpan), Convert.ToString(menu.Name), Convert.ToString(secondSpan));

        //                //ul name 
        //                TagBuilder ulChild = new TagBuilder("ul");
        //                ulChild.AddCssClass("dropdown-menu");


        //                foreach (sp_UserAccessPermissions_Result childMenu in childList)
        //                {

        //                    TagBuilder firstSpanchild = ITag("icon-task");

        //                    TagBuilder liChildaLink = AnchorLink(childMenu.Action, childMenu.Controller, false);
        //                    liChildaLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(firstSpanchild), Convert.ToString(childMenu.Name));

        //                    TagBuilder liChild = new TagBuilder("li") { InnerHtml = Convert.ToString(liChildaLink) };
        //                    //liChild.AddCssClass("nav-item dropdown");
        //                    liChild.AddCssClass("dropdown-item");

        //                    sbChild.Append(Convert.ToString(liChild));
        //                }

        //                ulChild.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}", Convert.ToString(sbChild));
        //                liWithChild.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(aLink), Convert.ToString(ulChild));
        //                sb.Append(Convert.ToString(liWithChild));
        //            }
        //            else
        //            {
        //                TagBuilder imageTag = ITag(menu.ImagePath + "  position-left");

        //                TagBuilder aLink = AnchorLink(menu.Action, menu.Controller, false);
        //                aLink.AddCssClass("nav-link");
        //                aLink.InnerHtml = string.Format("{0}{1}", Convert.ToString(imageTag), menu.Name);
        //                //aLink.MergeAttribute("class", "legitRipple");


        //                TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
        //                liWithChild.AddCssClass("nav-item");
        //                sb.Append(Convert.ToString(liWithChild));


        //                //TagBuilder firstSpan = ItalicTag(menu.ImagePath);

        //                //TagBuilder secondSpan = SpanTag("");
        //                //secondSpan.InnerHtml = Convert.ToString(menu.Name);

        //                //TagBuilder aLink = AnchorLink(menu.Action, menu.Controller, false);
        //                //aLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(firstSpan), Convert.ToString(secondSpan));

        //                //TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
        //                ////liWithChild.AddCssClass("sidenav-item");
        //                //sb.Append(Convert.ToString(liWithChild));
        //            }
        //        }

        //        ul.InnerHtml = sb.ToString();
        //        return MvcHtmlString.Create(ul.ToString());
        //    }

        //    return MvcHtmlString.Empty;
        //}

        public static MvcHtmlString GenerateMenu(this HtmlHelper helper)
        {
            List<sp_UserAccessPermissions_Result> parentMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId == null).OrderBy(item => item.DisplayOrder).ToList();

            List<sp_UserAccessPermissions_Result> childMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId != null).OrderBy(item => item.DisplayOrder).ToList();

            if (parentMenuList.Any())
            {
                //ul.MergeAttribute("class", "navigation navigation-main navigation-accordion");
               // ul.MergeAttribute("class", "navbar-nav mr-auto");
                //ul.MergeAttribute("id", "sideBar");

                StringBuilder sb = new StringBuilder();

                foreach (sp_UserAccessPermissions_Result menu in parentMenuList)
                {
                    List<sp_UserAccessPermissions_Result> childList = childMenuList.Where(x => x.ParentMenuId == menu.MenuId).ToList();

                    if (childList.Any())
                    {
                        StringBuilder sbChild = new StringBuilder();


                        //li name 
                        TagBuilder liWithChild = new TagBuilder("li");
                        liWithChild.AddCssClass("nav-item");
                        if (menu.MenuId != 8 && menu.MenuId != 18)
                        {
                            liWithChild.MergeAttribute("id", "menu_" + menu.Controller);
                        }

                        //Static a tag
                        TagBuilder firstSpan = ITag(menu.ImagePath);
                        //TagBuilder secondSpan = SpanTag("caret");

                        TagBuilder aLink = AnchorLink(null, null, true, menu.MenuId);
                        aLink.AddCssClass("nav-link");
                        aLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(firstSpan), Convert.ToString(menu.Name));

                        //ul name 
                        TagBuilder divChild = new TagBuilder("div");
                        divChild.AddCssClass("collapse");
                        if (menu.MenuId == 8)
                        {
                            divChild.GenerateId("collapseTwo");
                            liWithChild.MergeAttribute("id", "menu_Content");
                        }
                        if (menu.MenuId == 18)
                        {
                            divChild.GenerateId("collapseUtilities");
                            liWithChild.MergeAttribute("id", "menu_QuoteDetails");
                        }

                        TagBuilder divInner = new TagBuilder("div");
                        divInner.AddCssClass("collapse-inner");
                        foreach (sp_UserAccessPermissions_Result childMenu in childList)
                        {

                            TagBuilder firstSpanchild = ITag(childMenu.ImagePath);

                            TagBuilder liChildaLink = AnchorLink(childMenu.Action, childMenu.Controller, false, childMenu.MenuId);
                            liChildaLink.MergeAttribute("id", "menu_" + childMenu.Controller);
                            liChildaLink.AddCssClass("collapse-item");
                            liChildaLink.InnerHtml = string.Format("{0}{1}", Convert.ToString(firstSpanchild), Convert.ToString(childMenu.Name));

                            //TagBuilder liChild = new TagBuilder("li") { InnerHtml = Convert.ToString(liChildaLink) };
                            ////liChild.AddCssClass("nav-item dropdown");
                            //liChild.AddCssClass("collapse-item");

                            sbChild.Append(Convert.ToString(liChildaLink));
                        }

                        divInner.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}", Convert.ToString(sbChild));
                        // ulChild.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}", Convert.ToString(sbChild));
                        divChild.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}", Convert.ToString(divInner));
                        liWithChild.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(aLink), Convert.ToString(divChild));
                        sb.Append(Convert.ToString(liWithChild));
                    }
                    else
                    {
                        TagBuilder imageTag = ITag(menu.ImagePath);

                        TagBuilder aLink = AnchorLink(menu.Action, menu.Controller, false, menu.MenuId);
                        aLink.AddCssClass("nav-link");
                        aLink.InnerHtml = string.Format("{0}{1}", Convert.ToString(imageTag), menu.Name);
                        //aLink.MergeAttribute("class", "legitRipple");


                        TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
                        liWithChild.AddCssClass("nav-item");
                        liWithChild.MergeAttribute("id", "menu_" + menu.Controller);
                        sb.Append(Convert.ToString(liWithChild));


                        //TagBuilder firstSpan = ItalicTag(menu.ImagePath);

                        //TagBuilder secondSpan = SpanTag("");
                        //secondSpan.InnerHtml = Convert.ToString(menu.Name);

                        //TagBuilder aLink = AnchorLink(menu.Action, menu.Controller, false);
                        //aLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(firstSpan), Convert.ToString(secondSpan));

                        //TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
                        ////liWithChild.AddCssClass("sidenav-item");
                        //sb.Append(Convert.ToString(liWithChild));
                    }
                }

             //  ul.InnerHtml = sb.ToString();
                return MvcHtmlString.Create(sb.ToString());
            }

            return MvcHtmlString.Empty;
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="isParent"></param>
        /// <returns></returns>
        private static TagBuilder AnchorLink(string actionName, string controllerName, bool isParent,short? menuid)
        {

            TagBuilder aLink = new TagBuilder("a");

            if (isParent)
            {
                aLink.MergeAttribute("class", "collapsed");
                //aLink.MergeAttribute("data-action", "click-trigger");
                aLink.MergeAttribute("data-toggle", "collapse");
                if (menuid == 8)
                {
                    aLink.MergeAttribute("data-target", "#collapseTwo");
                    aLink.MergeAttribute("aria-controls", "collapseTwo");
                }
                if (menuid == 18)
                {
                    aLink.MergeAttribute("data-target", "#collapseUtilities");
                    aLink.MergeAttribute("aria-controls", "collapseUtilities");
                }
            }

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
            {
                aLink.MergeAttribute("href", "javascript:void(0);");
            }
            else
            {
              
                aLink.MergeAttribute("href", HtmlHelperExtensions.SiteRootPathUrl + "/" + controllerName + "/" + actionName);
            }
            return aLink;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static TagBuilder ITag(string className)
        {
            TagBuilder span = new TagBuilder("i");
            span.MergeAttribute("class", className);
            return span;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static TagBuilder SpanTag(string className)
        {
            TagBuilder span = new TagBuilder("span");
            span.MergeAttribute("class", className);
            return span;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static TagBuilder ItalicTag(string className)
        {
            TagBuilder span = new TagBuilder("i");
            span.MergeAttribute("class", className);
            return span;
        }
        public static string SiteRootPathUrl
        {
            get
            {
                string msRootUrl;
                if (HttpContext.Current.Request.ApplicationPath != null && string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath.Split('/')[1]))
                {
                    msRootUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host +
                                ":" +
                                HttpContext.Current.Request.Url.Port;
                }
                else
                {
                    msRootUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.ApplicationPath;
                }

                return msRootUrl;
            }
        }



    }
    //public class ReportHelper
    //{
    //    public static readonly string ReportApiController = HtmlHelperExtensions.SiteRootPathUrl + "/api/reportsapi/";

    //    public static readonly string ReportTemplate = HtmlHelperExtensions.SiteRootPathUrl + "/Template/telerikReportViewerTemplate.html";
    //}
}
