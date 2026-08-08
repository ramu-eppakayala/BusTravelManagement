using System;
using System.Web.Mvc;

namespace BusTravelManagement.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GlobalExceptionFilter));

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var ctlName = filterContext.RouteData?.Values["controller"]?.ToString();
            var actName = filterContext.RouteData?.Values["action"]?.ToString();
            log.Error($"Unhandled exception in {filterContext.Controller?.GetType()?.Name}.{actName}: {filterContext.Exception.Message}",
                      filterContext.Exception);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "An unexpected error occurred. Please try again.",
                        error = filterContext.Exception.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                return;
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.ExceptionHandled = true;
        }
    }
}
