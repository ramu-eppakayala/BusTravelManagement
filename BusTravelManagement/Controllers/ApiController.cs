using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class ApiController : Controller
    {
        private readonly ISearchService _searchService;
        private readonly IBookingService _bookingService;
        private readonly ICouponService _couponService;
        private readonly IReviewService _reviewService;
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public ApiController(ISearchService searchService, IBookingService bookingService,
            ICouponService couponService, IReviewService reviewService,
            IAuthService authService, IUnitOfWork unitOfWork)
        {
            _searchService = searchService;
            _bookingService = bookingService;
            _couponService = couponService;
            _reviewService = reviewService;
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public JsonResult SearchCities(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Json(new { results = new System.Collections.ArrayList() }, JsonRequestBehavior.AllowGet);

            var cities = _searchService.SearchCities(q);
            var results = cities.Select(c => new { id = c.Id, text = c.DisplayName });
            return Json(new { results }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchBuses(int from, int to, string date)
        {
            if (!System.DateTime.TryParse(date, out var journeyDate))
                return Json(new { success = false, message = "Invalid date" }, JsonRequestBehavior.AllowGet);

            var model = new SearchViewModel
            {
                SourceCityId = from,
                DestinationCityId = to,
                JourneyDate = journeyDate
            };

            var results = _searchService.SearchBuses(model);
            return Json(new { success = true, data = results }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAvailableSeats(int scheduleId, string date)
        {
            if (!System.DateTime.TryParse(date, out var journeyDate))
                return Json(new { success = false, message = "Invalid date" }, JsonRequestBehavior.AllowGet);

            var seats = _searchService.GetAvailableSeats(scheduleId, journeyDate);
            return Json(new { success = true, data = seats }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateCoupon(string code, decimal amount)
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { isValid = false, message = "Please login" });

            var result = _couponService.ValidateCoupon(code, amount, userId.Value);
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetBusReviews(int busId)
        {
            var reviews = _reviewService.GetBusReviews(busId);
            var stats = _reviewService.GetBusReviewStats(busId);
            return Json(new { success = true, reviews, stats }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetUserProfile()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "Not authenticated" }, JsonRequestBehavior.AllowGet);

            var profile = _authService.GetProfile(userId.Value);
            return Json(new { success = true, data = profile }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetNotifications()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var notificationService = DependencyResolver.Current.GetService<INotificationService>();
            var notifications = notificationService.GetUserNotifications(userId.Value);
            var unreadCount = notificationService.GetUnreadCount(userId.Value);

            return Json(new
            {
                success = true,
                data = notifications,
                unreadCount
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult MarkNotificationRead(int notificationId)
        {
            var notificationService = DependencyResolver.Current.GetService<INotificationService>();
            notificationService.MarkAsRead(notificationId);
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public JsonResult MarkAllNotificationsRead()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false });

            var notificationService = DependencyResolver.Current.GetService<INotificationService>();
            notificationService.MarkAllAsRead(userId.Value);
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetPopularCities()
        {
            var cities = _unitOfWork.Cities.GetPopularCities();
            var result = cities.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                state = c.State,
                display = $"{c.Name}, {c.State}"
            });
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBookingStatus(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var booking = _bookingService.GetBookingDetail(bookingNumber);
            if (booking == null)
                return Json(new { success = false, message = "Booking not found" }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                data = new
                {
                    bookingNumber = booking.BookingNumber,
                    status = booking.BookingStatus,
                    journeyDate = booking.JourneyDate.ToString("yyyy-MM-dd"),
                    isCancellable = booking.IsCancellable
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFare(int scheduleId, int sourceStopId, int destStopId, int seats, string coupon = null)
        {
            var fare = _searchService.CalculateFare(scheduleId, sourceStopId, destStopId, seats, coupon);
            return Json(new { success = true, data = fare }, JsonRequestBehavior.AllowGet);
        }
    }
}
