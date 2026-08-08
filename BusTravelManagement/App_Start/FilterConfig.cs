using System.Web.Mvc;

namespace BusTravelManagement.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute
            {
                View = "Error",
                ExceptionType = typeof(System.Exception),
                Master = null
            });
            filters.Add(new Filters.GlobalExceptionFilter());
            filters.Add(new Filters.AuditLogFilter());
        }
    }
}
