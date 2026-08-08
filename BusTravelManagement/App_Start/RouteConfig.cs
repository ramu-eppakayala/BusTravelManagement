using System.Web.Mvc;
using System.Web.Routing;

namespace BusTravelManagement.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search/{sourceCity}/{destinationCity}/{journeyDate}",
                defaults: new { controller = "Search", action = "Index" }
            );

            routes.MapRoute(
                name: "SeatLayout",
                url: "booking/seats/{scheduleId}/{journeyDate}",
                defaults: new { controller = "Booking", action = "SelectSeats" }
            );

            routes.MapRoute(
                name: "BookingConfirmation",
                url: "booking/confirmation/{bookingNumber}",
                defaults: new { controller = "Booking", action = "Confirmation" }
            );

            routes.MapRoute(
                name: "TicketView",
                url: "ticket/{bookingNumber}",
                defaults: new { controller = "Booking", action = "ViewTicket" }
            );

            routes.MapRoute(
                name: "OperatorDashboard",
                url: "operator/{action}/{id}",
                defaults: new { controller = "Operator", action = "Dashboard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AdminArea",
                url: "admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ApiRoutes",
                url: "api/{action}/{param}",
                defaults: new { controller = "Api", action = "Index", param = UrlParameter.Optional }
            );
        }
    }
}
