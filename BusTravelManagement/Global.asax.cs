using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BusTravelManagement.App_Start;

namespace BusTravelManagement
{
    public class MvcApplication : HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            try
            {
                log.Info("Application starting...");

                // Anti-forgery tokens must derive a stable unique claim from the
                // authenticated identity. Every login emits ClaimTypes.NameIdentifier,
                // so pin it explicitly. Without this, any identity that predates that
                // claim (e.g. stale cookies from earlier auth builds) makes
                // @Html.AntiForgeryToken() throw an InvalidOperationException.
                System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier =
                    System.Security.Claims.ClaimTypes.NameIdentifier;

                // Initialize configurations
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                LoggingConfig.Configure();
                MapperConfig.Configure();
                DependencyInjectionConfig.RegisterComponents();

                // Initialize database
                DatabaseConfig.Initialize();

                log.Info("Application started successfully.");
            }
            catch (Exception ex)
            {
                log.Fatal("Application failed to start", ex);
                throw;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                log.Error("Unhandled application error", exception);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Set culture
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-IN");
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("en-IN");
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // Cleanup
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["Init"] = DateTime.UtcNow;
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Cleanup session data
        }
    }
}
