using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BusTravelManagement.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var errors = filterContext.Controller.ViewData.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            Errors = e.Value.Errors.Select(x => x.ErrorMessage)
                        });

                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = "Validation failed",
                            errors = errors
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    filterContext.Controller.ViewBag.ValidationErrors = filterContext.Controller.ViewData.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors.Select(x => x.ErrorMessage))
                        .ToList();

                    var controller = filterContext.RouteData.Values["controller"];
                    var action = filterContext.RouteData.Values["action"];

                    filterContext.Result = new ViewResult
                    {
                        ViewName = filterContext.ActionDescriptor.ActionName,
                        ViewData = filterContext.Controller.ViewData,
                        TempData = filterContext.Controller.TempData
                    };
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
