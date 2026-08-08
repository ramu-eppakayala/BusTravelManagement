using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ISearchService searchService, IUnitOfWork unitOfWork)
        {
            _searchService = searchService;
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            var popularCities = _unitOfWork.Cities.GetPopularCities();
            ViewBag.PopularCities = popularCities;
            ViewBag.SourceCities = popularCities;

            var topOperators = _unitOfWork.Operators.Query()
                .Where(o => o.IsActive && o.IsVerified)
                .Take(6)
                .Select(o => new { o.CompanyName, o.LogoUrl, o.City })
                .ToList();
            ViewBag.TopOperators = topOperators;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Header()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = GetCurrentUserId();
                if (userId.HasValue)
                {
                    ViewBag.UnreadNotifications = _unitOfWork.Notifications.GetUnreadCount(userId.Value);
                }
            }
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult _Footer()
        {
            return PartialView();
        }

        private int? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var authService = DependencyResolver.Current.GetService<IAuthService>();
                return authService?.GetCurrentUserId();
            }
            return null;
        }
    }
}
