using System;
using System.Linq;
using System.Web.Mvc;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;
        private readonly ICouponService _couponService;
        private readonly ITicketService _ticketService;
        private readonly IAuthService _authService;

        public BookingController(IBookingService bookingService, IPaymentService paymentService,
            ICouponService couponService, ITicketService ticketService, IAuthService authService)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
            _couponService = couponService;
            _ticketService = ticketService;
            _authService = authService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult SelectSeats(int scheduleId, DateTime journeyDate)
        {
            var searchService = DependencyResolver.Current.GetService<ISearchService>();
            var busDetail = searchService.GetBusDetail(scheduleId, journeyDate);
            if (busDetail == null)
                return HttpNotFound();

            ViewBag.SeatLayout = searchService.GetAvailableSeats(scheduleId, journeyDate);
            return View(busDetail);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SelectSeats(BookingRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var searchService = DependencyResolver.Current.GetService<ISearchService>();
                var busDetail = searchService.GetBusDetail(model.ScheduleId, model.JourneyDate);
                ViewBag.SeatLayout = searchService.GetAvailableSeats(model.ScheduleId, model.JourneyDate);
                return View(busDetail);
            }

            var result = _bookingService.InitiateBooking(model);
            if (result.Success)
            {
                Session["BookingData"] = model;
                Session["BookingNumber"] = result.BookingNumber;
                return RedirectToAction("PassengerDetails");
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("SelectSeats", new { scheduleId = model.ScheduleId, journeyDate = model.JourneyDate });
        }

        [HttpGet]
        [Authorize]
        public ActionResult PassengerDetails()
        {
            var bookingNumber = Session["BookingNumber"] as string;
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var bookingData = Session["BookingData"] as BookingRequestViewModel;
            if (bookingData == null)
                return RedirectToAction("Index", "Home");

            var model = new ConfirmBookingViewModel
            {
                BookingNumber = bookingNumber,
                ScheduleId = bookingData.ScheduleId,
                Passengers = new System.Collections.Generic.List<PassengerDetailViewModel>()
            };

            foreach (var seatId in bookingData.SeatLayoutIds)
            {
                model.Passengers.Add(new PassengerDetailViewModel
                {
                    SeatLayoutId = seatId,
                    Gender = "Male",
                    Age = 25
                });
            }

            var searchService = DependencyResolver.Current.GetService<ISearchService>();
            var busDetail = searchService.GetBusDetail(bookingData.ScheduleId, bookingData.JourneyDate);
            var fare = searchService.CalculateFare(bookingData.ScheduleId, bookingData.SourceStopId,
                bookingData.DestinationStopId, bookingData.SeatLayoutIds.Count);
            if (busDetail != null)
            {
                model.OperatorName = busDetail.OperatorName;
                model.BusNumber = busDetail.BusNumber;
                model.SourceCity = busDetail.SourceCity;
                model.DestinationCity = busDetail.DestinationCity;
            }
            model.BaseFare = fare.PerSeatFare;
            model.ConvenienceFee = fare.ConvenienceFee;
            model.Tax = fare.TaxAmount;
            model.TotalAmount = fare.NetAmount;
            model.SelectedSeats = searchService.GetAvailableSeats(bookingData.ScheduleId, bookingData.JourneyDate)
                .Where(s => bookingData.SeatLayoutIds.Contains(s.SeatLayoutId))
                .ToList();
            if (busDetail != null && busDetail.BoardingPoints != null)
            {
                var boardingPoint = busDetail.BoardingPoints.FirstOrDefault(p => p.Id == bookingData.BoardingPointId);
                if (boardingPoint != null)
                {
                    model.BoardingPointName = boardingPoint.Name;
                    model.BoardingPointTime = boardingPoint.PickupTime;
                }
            }

            var userId = _authService.GetCurrentUserId();
            if (userId.HasValue)
            {
                var profile = _authService.GetProfile(userId.Value);
                if (profile != null)
                {
                    model.ContactName = $"{profile.FirstName} {profile.LastName}";
                    model.ContactPhone = profile.PhoneNumber;
                    model.ContactEmail = profile.Email;
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult PassengerDetails(ConfirmBookingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Session["ConfirmData"] = model;
            return RedirectToAction("Payment");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Payment()
        {
            var confirmData = Session["ConfirmData"] as ConfirmBookingViewModel;
            if (confirmData == null)
                return RedirectToAction("Index", "Home");

            var bookingNumber = Session["BookingNumber"] as string;
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var searchService = DependencyResolver.Current.GetService<ISearchService>();
            var bookingData = Session["BookingData"] as BookingRequestViewModel;
            if (bookingData == null)
                return RedirectToAction("Index", "Home");

            var fare = searchService.CalculateFare(bookingData.ScheduleId, bookingData.SourceStopId,
                bookingData.DestinationStopId, bookingData.SeatLayoutIds.Count, confirmData.CouponCode);
            var busDetail = searchService.GetBusDetail(bookingData.ScheduleId, bookingData.JourneyDate);

            return View(new PaymentRequestViewModel
            {
                BookingNumber = bookingNumber,
                Amount = fare.NetAmount,
                PaymentMethod = "Credit Card",
                PaymentGateway = "Mock",
                OperatorName = busDetail != null ? busDetail.OperatorName : null,
                BusNumber = busDetail != null ? busDetail.BusNumber : null,
                SourceCity = busDetail != null ? busDetail.SourceCity : null,
                DestinationCity = busDetail != null ? busDetail.DestinationCity : null,
                TravelDate = bookingData.JourneyDate,
                DepartureTime = busDetail != null ? busDetail.DepartureTime : null,
                ArrivalTime = busDetail != null ? busDetail.ArrivalTime : null,
                SelectedSeats = searchService.GetAvailableSeats(bookingData.ScheduleId, bookingData.JourneyDate)
                    .Where(s => bookingData.SeatLayoutIds.Contains(s.SeatLayoutId))
                    .Select(s => s.SeatNumber)
                    .ToList()
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(PaymentRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var confirmData = Session["ConfirmData"] as ConfirmBookingViewModel;
            if (confirmData == null)
                return RedirectToAction("Index", "Home");

            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            // Process payment
            var paymentResult = _paymentService.ProcessPayment(model);
            if (!paymentResult.Success)
            {
                ViewBag.ErrorMessage = paymentResult.Message;
                return View(model);
            }

            // Confirm booking
            confirmData.PaymentMethod = model.PaymentMethod;
            var bookingResult = _bookingService.ConfirmBooking(userId.Value, confirmData);

            if (bookingResult.Success)
            {
                // Generate ticket
                try
                {
                    _ticketService.GenerateTicket(bookingResult.BookingNumber);
                }
                catch { /* Ticket generation failure is non-critical */ }

                Session.Remove("BookingData");
                Session.Remove("ConfirmData");
                Session.Remove("BookingNumber");

                return RedirectToAction("Confirmation", new { bookingNumber = bookingResult.BookingNumber });
            }

            ViewBag.ErrorMessage = bookingResult.Message;
            return View(model);
        }

        [HttpGet]
        public ActionResult Confirmation(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var booking = _bookingService.GetBookingConfirmation(bookingNumber);
            if (booking == null)
                return HttpNotFound();

            return View(booking);
        }

        [HttpGet]
        public ActionResult ViewTicket(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var ticket = _ticketService.GetTicket(bookingNumber);
            if (ticket == null)
                return HttpNotFound();

            return View(ticket);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Cancel(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var booking = _bookingService.GetBookingDetail(bookingNumber);
            if (booking == null || !booking.IsCancellable)
                return HttpNotFound();

            return View(booking);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(string bookingNumber, string reason)
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var result = _bookingService.CancelBooking(userId.Value, bookingNumber, reason);
            if (result.Success)
            {
                TempData["SuccessMessage"] = $"Booking cancelled. Refund: ₹{result.RefundAmount:N2}";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Detail", new { bookingNumber });
        }

        [HttpGet]
        public ActionResult Detail(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            var booking = _bookingService.GetBookingDetail(bookingNumber);
            if (booking == null)
                return HttpNotFound();

            return View(booking);
        }

        [HttpGet]
        public ActionResult DownloadTicket(string bookingNumber)
        {
            if (string.IsNullOrEmpty(bookingNumber))
                return RedirectToAction("Index", "Home");

            try
            {
                var pdfBytes = _ticketService.GenerateTicketPdf(bookingNumber);
                return File(pdfBytes, "application/pdf", $"Ticket_{bookingNumber}.pdf");
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to generate PDF. Please try again.";
                return RedirectToAction("ViewTicket", new { bookingNumber });
            }
        }

        [HttpPost]
        public JsonResult ValidateCoupon(string code, decimal amount)
        {
            var userId = _authService.GetCurrentUserId();
            if (!userId.HasValue)
                return Json(new { isValid = false, message = "Please login to use coupons." });

            var result = _couponService.ValidateCoupon(code, amount, userId.Value);
            return Json(result);
        }

        [HttpPost]
        public JsonResult ValidatePassengerAge(int age)
        {
            if (age < 1 || age > 120)
                return Json(new { valid = false, message = "Age must be between 1 and 120." });
            return Json(new { valid = true });
        }
    }
}
