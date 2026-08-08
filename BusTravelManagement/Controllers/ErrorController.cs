using System.Web.Mvc;

namespace BusTravelManagement.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult General()
        {
            Response.StatusCode = 500;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }

        public ActionResult BadRequest()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult Maintenance()
        {
            Response.StatusCode = 503;
            var maintenanceMode = System.Configuration.ConfigurationManager.AppSettings["app:MaintenanceMode"];
            if (maintenanceMode != "true")
                return RedirectToAction("Index", "Home");

            ViewBag.Message = System.Configuration.ConfigurationManager.AppSettings["app:MaintenanceMessage"];
            return View();
        }
    }
}
