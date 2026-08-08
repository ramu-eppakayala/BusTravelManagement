using System;
using System.Web.Mvc;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IAuthService _authService;

        public ReviewController(IReviewService reviewService, IAuthService authService)
        {
            _reviewService = reviewService;
            _authService = authService;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(SubmitReviewViewModel model)
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { success = false, message = "Please login to submit a review." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid review data." });

            try
            {
                _reviewService.SubmitReview(userId.Value, model);
                return Json(new { success = true, message = "Review submitted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetBusReviews(int busId)
        {
            var reviews = _reviewService.GetBusReviews(busId);
            var stats = _reviewService.GetBusReviewStats(busId);
            return Json(new { reviews, stats }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ReportAbuse(int reviewId, string reason)
        {
            try
            {
                _reviewService.ReportAbuse(reviewId, reason);
                return Json(new { success = true, message = "Report submitted. We will review it." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
