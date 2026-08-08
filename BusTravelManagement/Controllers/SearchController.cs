using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IReviewService _reviewService;

        public SearchController(ISearchService searchService, IReviewService reviewService)
        {
            _searchService = searchService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public ActionResult Index(int sourceCityId, int destinationCityId, DateTime journeyDate, int page = 1)
        {
            var model = new SearchViewModel
            {
                SourceCityId = sourceCityId,
                DestinationCityId = destinationCityId,
                JourneyDate = journeyDate,
                Page = page < 1 ? 1 : page
            };

            var results = _searchService.SearchBuses(model);
            return View(results);
        }

        [HttpGet]
        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", new
            {
                sourceCityId = model.SourceCityId,
                destinationCityId = model.DestinationCityId,
                journeyDate = model.JourneyDate.ToString("yyyy-MM-dd")
            });
        }

        [HttpGet]
        public ActionResult BusDetail(int scheduleId, DateTime journeyDate)
        {
            var busDetail = _searchService.GetBusDetail(scheduleId, journeyDate);
            if (busDetail == null)
                return HttpNotFound();

            ViewBag.Reviews = _reviewService.GetBusReviews(busDetail.BusId);
            ViewBag.ReviewStats = _reviewService.GetBusReviewStats(busDetail.BusId);
            return View(busDetail);
        }

        [HttpGet]
        public JsonResult SearchCities(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new System.Collections.Generic.List<object>(), JsonRequestBehavior.AllowGet);

            var cities = _searchService.SearchCities(term);
            var result = cities.Select(c => new
            {
                id = c.Id,
                text = c.DisplayName
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPopularCities()
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var cities = unitOfWork.Cities.GetPopularCities();
            var result = cities.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                state = c.State,
                displayName = c.DisplayName
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableSeats(int scheduleId, DateTime journeyDate)
        {
            var seats = _searchService.GetAvailableSeats(scheduleId, journeyDate);
            return Json(seats, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CalculateFare(int scheduleId, int sourceStopId, int destStopId,
                                         int seatCount, string couponCode = null)
        {
            var fare = _searchService.CalculateFare(scheduleId, sourceStopId, destStopId, seatCount, couponCode);
            return Json(fare);
        }
    }
}
