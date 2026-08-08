using System;
using System.Web.Mvc;

namespace BusTravelManagement.Filters
{
    public class AuditLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"]?.ToString();
            var action = filterContext.RouteData.Values["action"]?.ToString();

            var log = log4net.LogManager.GetLogger("AuditLog");
            log4net.ThreadContext.Properties["Action"] = $"{controller}.{action}";
            log4net.ThreadContext.Properties["EntityType"] = controller;
            log4net.ThreadContext.Properties["UserId"] = 0;
            log4net.ThreadContext.Properties["IpAddress"] = filterContext.HttpContext.Request.UserHostAddress;
            log4net.ThreadContext.Properties["UserAgent"] = filterContext.HttpContext.Request.UserAgent;

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            log4net.ThreadContext.Properties.Remove("Action");
            log4net.ThreadContext.Properties.Remove("EntityType");
            log4net.ThreadContext.Properties.Remove("UserId");
            log4net.ThreadContext.Properties.Remove("IpAddress");
            log4net.ThreadContext.Properties.Remove("UserAgent");

            base.OnActionExecuted(filterContext);
        }
    }
}
