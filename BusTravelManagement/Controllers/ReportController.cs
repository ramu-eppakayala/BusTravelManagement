using System;
using System.Web.Mvc;
using BusTravelManagement.Filters;
using BusTravelManagement.Services.Interfaces;
using BusTravelManagement.Utilities.Constants;

namespace BusTravelManagement.Controllers
{
    [RoleAuthorize(RoleConstants.SuperAdmin, RoleConstants.Admin, RoleConstants.Operator)]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public ActionResult Daily(DateTime? date)
        {
            var reportDate = date ?? DateTime.Today;
            var report = _reportService.GetDailyReport(reportDate);
            return View(report);
        }

        public ActionResult Monthly(int? year, int? month)
        {
            var y = year ?? DateTime.Today.Year;
            var m = month ?? DateTime.Today.Month;
            var report = _reportService.GetMonthlyReport(y, m);
            return View(report);
        }

        public ActionResult Yearly(int? year)
        {
            var y = year ?? DateTime.Today.Year;
            var report = _reportService.GetYearlyReport(y);
            return View(report);
        }

        public ActionResult Bookings(DateTime? fromDate, DateTime? toDate, string status = null)
        {
            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var to = toDate ?? DateTime.Today;
            var report = _reportService.GetBookingReport(from, to, status);
            return View(report);
        }

        public ActionResult Cancellations(DateTime? fromDate, DateTime? toDate)
        {
            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var to = toDate ?? DateTime.Today;
            var report = _reportService.GetCancellationReport(from, to);
            return View(report);
        }

        public ActionResult Operator(int? operatorId, DateTime? fromDate, DateTime? toDate)
        {
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var to = toDate ?? DateTime.Today;
            var report = _reportService.GetOperatorReport(operatorId.Value, from, to);
            return View(report);
        }

        public ActionResult Customer(int? userId, DateTime? fromDate, DateTime? toDate)
        {
            if (!userId.HasValue)
                return RedirectToAction("Index", "Home");

            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var to = toDate ?? DateTime.Today;
            var report = _reportService.GetCustomerReport(userId.Value, from, to);
            return View(report);
        }

        public ActionResult Occupancy(int? busId, DateTime? fromDate, DateTime? toDate)
        {
            if (!busId.HasValue)
                return RedirectToAction("Index", "Home");

            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var to = toDate ?? DateTime.Today;
            var report = _reportService.GetOccupancyReport(busId.Value, from, to);
            return View(report);
        }
    }
}
