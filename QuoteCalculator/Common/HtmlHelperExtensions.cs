using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuoteCalculator.Common
{
    public class HtmlHelperExtensions
    {
        //public static MvcHtmlString GenerateMenu(this HtmlHelper helper)
        //{
        //    //List<sp_UserAccessPermissions_Result> parentMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId == null).OrderBy(item => item.DisplayOrder).ToList();

        //    //List<sp_UserAccessPermissions_Result> childMenuList = SessionHelper.UserAccessPermissions.Where(x => x.ParentMenuId != null).OrderBy(item => item.DisplayOrder).ToList();

        //    //if (parentMenuList.Any())
        //    //{
        //        TagBuilder ul = new TagBuilder("ul");
        //        //ul.MergeAttribute("class", "navigation navigation-main navigation-accordion");
        //        ul.MergeAttribute("class", "nav navbar-nav navbar-nav-material");
        //        ul.MergeAttribute("id", "sideBar");

        //        StringBuilder sb = new StringBuilder();

        //        foreach (sp_UserAccessPermissions_Result menu in parentMenuList)
        //        {
        //            List<sp_UserAccessPermissions_Result> childList = childMenuList.Where(x => x.ParentMenuId == menu.MenuId).ToList();

        //            if (childList.Any())
        //            {
        //                StringBuilder sbChild = new StringBuilder();


        //                //li name 
        //                TagBuilder liWithChild = new TagBuilder("li");
        //                liWithChild.AddCssClass("dropdown");


        //                //Static a tag
        //                TagBuilder firstSpan = ITag("icon-make-group position-left");
        //                TagBuilder secondSpan = SpanTag("caret");

        //                TagBuilder aLink = AnchorLink(null, null, true);
        //                aLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Convert.ToString(firstSpan), Convert.ToString(menu.Name), Convert.ToString(secondSpan));

        //                //ul name 
        //                TagBuilder ulChild = new TagBuilder("ul");
        //                ulChild.AddCssClass("dropdown-menu width-250");


        //                foreach (sp_UserAccessPermissions_Result childMenu in childList)
        //                {

        //                    TagBuilder firstSpanchild = ITag("icon-task");

        //                    TagBuilder liChildaLink = AnchorLink(childMenu.Action, childMenu.Controller, false);
        //                    liChildaLink.InnerHtml = string.Format(CultureInfo.InvariantCulture, "{0}{1}", Convert.ToString(firstSpanchild), Convert.ToString(childMenu.Name));

        //                    TagBuilder liChild = new TagBuilder("li") { InnerHtml = Convert.ToString(liChildaLink) };
        //                    liChild.AddCssClass("dropdown-submenu");

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
        //                aLink.InnerHtml = string.Format("{0}{1}", Convert.ToString(imageTag), menu.Name);
        //                //aLink.MergeAttribute("class", "legitRipple");


        //                TagBuilder liWithChild = new TagBuilder("li") { InnerHtml = Convert.ToString(aLink) };
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
        //   // }

        //    return MvcHtmlString.Empty;
        //}
    }
}