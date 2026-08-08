using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Filters;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;
using BusTravelManagement.Utilities.Constants;

namespace BusTravelManagement.Controllers
{
    [RoleAuthorize(RoleConstants.Operator)]
    public class OperatorController : Controller
    {
        private readonly IOperatorService _operatorService;
        private readonly IBusService _busService;
        private readonly IAuthService _authService;

        public OperatorController(IOperatorService operatorService, IBusService busService, IAuthService authService)
        {
            _operatorService = operatorService;
            _busService = busService;
            _authService = authService;
        }

        public ActionResult Dashboard()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            var dashboard = _operatorService.GetDashboard(operatorId.Value);
            return View(dashboard);
        }

        public ActionResult Buses()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            var buses = _busService.GetOperatorBuses(operatorId.Value);
            return View(buses);
        }

        [HttpGet]
        public ActionResult CreateBus()
        {
            ViewBag.BusTypes = GetBusTypesSelectList();
            ViewBag.Amenities = GetAmenitiesSelectList();
            return View(new CreateBusViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBus(CreateBusViewModel model)
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.BusTypes = GetBusTypesSelectList();
                ViewBag.Amenities = GetAmenitiesSelectList();
                return View(model);
            }

            try
            {
                _busService.CreateBus(operatorId.Value, model);
                TempData["SuccessMessage"] = "Bus created successfully.";
                return RedirectToAction("Buses");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.BusTypes = GetBusTypesSelectList();
                ViewBag.Amenities = GetAmenitiesSelectList();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleBusStatus(int busId)
        {
            _busService.ToggleStatus(busId);
            return RedirectToAction("Buses");
        }

        [HttpGet]
        public ActionResult SeatLayout(int busId)
        {
            var layout = _busService.GetSeatLayout(busId);
            if (layout == null)
                return HttpNotFound();
            return View(layout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SeatLayout(int busId, SeatLayoutViewModel model)
        {
            _busService.UpdateSeatLayout(busId, model);
            TempData["SuccessMessage"] = "Seat layout updated.";
            return RedirectToAction("SeatLayout", new { busId });
        }

        public ActionResult Schedules()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            ViewBag.Buses = GetOperatorBusesSelectList(operatorId.Value);
            ViewBag.Routes = GetRoutesSelectList();
            var schedules = _operatorService.GetSchedules(operatorId.Value);
            return View(schedules);
        }

        [HttpGet]
        public ActionResult CreateSchedule()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            ViewBag.Buses = GetOperatorBusesSelectList(operatorId.Value);
            ViewBag.Routes = GetRoutesSelectList();
            return View(new CreateScheduleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSchedule(CreateScheduleViewModel model)
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Buses = GetOperatorBusesSelectList(operatorId.Value);
                ViewBag.Routes = GetRoutesSelectList();
                return View(model);
            }

            try
            {
                _operatorService.CreateSchedule(operatorId.Value, model);
                TempData["SuccessMessage"] = "Schedule created successfully.";
                return RedirectToAction("Schedules");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.Buses = GetOperatorBusesSelectList(operatorId.Value);
                ViewBag.Routes = GetRoutesSelectList();
                return View(model);
            }
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            var profile = _operatorService.GetProfile(operatorId.Value);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(OperatorProfileViewModel model)
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            _operatorService.UpdateProfile(operatorId.Value, model);
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        public ActionResult BookingReports()
        {
            var operatorId = GetCurrentOperatorId();
            if (!operatorId.HasValue)
                return RedirectToAction("Index", "Home");

            var reportService = DependencyResolver.Current.GetService<IReportService>();
            var report = reportService.GetOperatorReport(operatorId.Value,
                DateTime.Today.AddDays(-30), DateTime.Today);
            return View(report);
        }

        public ActionResult RevenueReports()
        {
            return View();
        }

        private int? GetCurrentOperatorId()
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue) return null;

            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var operatorEntity = unitOfWork.Operators.GetByUserId(userId.Value);
            return operatorEntity?.Id;
        }

        private SelectList GetBusTypesSelectList()
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var types = unitOfWork.Query<Models.Entities.BusType>().Where(t => t.IsActive).ToList();
            return new SelectList(types, "Id", "Name");
        }

        private SelectList GetAmenitiesSelectList()
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var amenities = unitOfWork.Query<Models.Entities.Amenity>().Where(a => a.IsActive).ToList();
            return new SelectList(amenities, "Id", "Name");
        }

        private SelectList GetOperatorBusesSelectList(int operatorId)
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var buses = unitOfWork.Query<Models.Entities.Bus>()
                .Where(b => b.OperatorId == operatorId && b.IsActive && !b.IsDeleted)
                .ToList();
            return new SelectList(buses, "Id", "BusNumber");
        }

        private SelectList GetRoutesSelectList()
        {
            var unitOfWork = DependencyResolver.Current.GetService<Repositories.Interfaces.IUnitOfWork>();
            var routes = unitOfWork.Query<Models.Entities.Route>()
                .Where(r => r.IsActive)
                .Select(r => new
                {
                    r.Id,
                    Name = r.SourceCity.Name + " → " + r.DestinationCity.Name
                })
                .ToList();
            return new SelectList(routes, "Id", "Name");
        }
    }
}
