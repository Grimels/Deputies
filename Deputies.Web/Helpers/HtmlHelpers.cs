using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Deputies.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper,
                                            string linkText,
                                            string actionName,
                                            string controllerName
                                            )
        {

            string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            if ((actionName == currentAction && controllerName == currentController)
                || (controllerName == currentController && currentController == "Deputy")
                || (controllerName == currentController && currentController == "Analytics"))
            {
                return new MvcHtmlString("<li class=\"active\">" + htmlHelper.ActionLink(linkText, actionName, controllerName).ToString() + "</li>");
            }

            return new MvcHtmlString("<li>" + htmlHelper.ActionLink(linkText, actionName, controllerName).ToString() + "</li>");
        }
    }
}