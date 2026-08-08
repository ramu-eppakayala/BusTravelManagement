using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Filters;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;
using BusTravelManagement.Utilities.Constants;

namespace BusTravelManagement.Controllers
{
    [RoleAuthorize(RoleConstants.SuperAdmin, RoleConstants.Admin)]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;

        public AdminController(IAdminService adminService, IReportService reportService)
        {
            _adminService = adminService;
            _reportService = reportService;
        }

        public ActionResult Dashboard()
        {
            var dashboard = _adminService.GetDashboard();
            return View(dashboard);
        }

        public ActionResult Users(int page = 1)
        {
            var users = _adminService.GetUsers(page);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleUserStatus(int userId)
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var user = unitOfWork.Users.GetById(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.UtcNow;
                unitOfWork.SaveChanges();
                TempData["SuccessMessage"] = $"User {(user.IsActive ? "activated" : "deactivated")}.";
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRole(int userId, string roleName)
        {
            _adminService.AssignRole(userId, roleName);
            TempData["SuccessMessage"] = "Role assigned successfully.";
            return RedirectToAction("Users");
        }

        public ActionResult Operators(int page = 1)
        {
            var operators = _adminService.GetOperators(page);
            return View(operators);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveOperator(int operatorId)
        {
            _adminService.ApproveOperator(operatorId);
            TempData["SuccessMessage"] = "Operator approved successfully.";
            return RedirectToAction("Operators");
        }

        public ActionResult Buses(int page = 1)
        {
            var buses = _adminService.GetAllBuses(page);
            return View(buses);
        }

        public ActionResult Coupons()
        {
            var coupons = _adminService.GetCoupons();
            return View(coupons);
        }

        [HttpGet]
        public ActionResult CreateCoupon()
        {
            return View(new CreateCouponViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCoupon(CreateCouponViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _adminService.CreateCoupon(model);
                TempData["SuccessMessage"] = "Coupon created successfully.";
                return RedirectToAction("Coupons");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleCoupon(int couponId)
        {
            _adminService.ToggleCouponStatus(couponId);
            return RedirectToAction("Coupons");
        }

        public ActionResult Reviews()
        {
            var reviews = _adminService.GetPendingReviews();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveReview(int reviewId)
        {
            _adminService.ApproveReview(reviewId);
            return RedirectToAction("Reviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectReview(int reviewId)
        {
            _adminService.RejectReview(reviewId);
            return RedirectToAction("Reviews");
        }

        public ActionResult AuditLogs(int page = 1)
        {
            var auditService = DependencyResolver.Current.GetService<IAuditService>();
            var logs = auditService.GetLogs(page);
            return View(logs);
        }

        public ActionResult Reports()
        {
            var dailyReport = _reportService.GetDailyReport(DateTime.Today);
            var monthlyReport = _reportService.GetMonthlyReport(DateTime.Today.Year, DateTime.Today.Month);
            ViewBag.DailyReport = dailyReport;
            ViewBag.MonthlyReport = monthlyReport;
            return View();
        }
    }
}
