using System.Collections.Generic;
using BusTravelManagement.Models.ViewModels;

namespace BusTravelManagement.Services.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<CitySearchResult> SearchCities(string term);
        SearchResultViewModel SearchBuses(SearchViewModel model);
        BusDetailViewModel GetBusDetail(int scheduleId, System.DateTime journeyDate);
        IEnumerable<SeatViewModel> GetAvailableSeats(int scheduleId, System.DateTime journeyDate);
        FareBreakdownViewModel CalculateFare(int scheduleId, int sourceStopId, int destStopId,
                                              int seatCount, string couponCode = null);
    }

    public interface IBookingService
    {
        BookingResultViewModel InitiateBooking(BookingRequestViewModel model);
        BookingResultViewModel ConfirmBooking(int userId, ConfirmBookingViewModel model);
        BookingConfirmationViewModel GetBookingConfirmation(string bookingNumber);
        BookingHistoryViewModel GetUserBookings(int userId, int page = 1, int pageSize = 10);
        CancelBookingResult CancelBooking(int userId, string bookingNumber, string reason);
        BookingDetailViewModel GetBookingDetail(string bookingNumber);
        void ReleaseHeldSeats(int scheduleId, System.DateTime journeyDate);
    }

    public interface IPaymentService
    {
        PaymentResultViewModel ProcessPayment(PaymentRequestViewModel model);
        PaymentStatusViewModel CheckPaymentStatus(string paymentReference);
        RefundResultViewModel ProcessRefund(string bookingNumber, string reason);
    }

    public interface ICouponService
    {
        CouponValidationResult ValidateCoupon(string code, decimal bookingAmount, int userId);
    }

    public interface ITicketService
    {
        TicketViewModel GenerateTicket(string bookingNumber);
        TicketViewModel GetTicket(string bookingNumber);
        string GenerateQRCode(string data);
        byte[] GenerateTicketPdf(string bookingNumber);
        void SendTicketEmail(string bookingNumber);
    }

    public interface INotificationService
    {
        void SendEmail(string to, string subject, string body);
        void SendSms(string phone, string message);
        void SendSystemNotification(int userId, string title, string message,
                                    string referenceType = null, int? referenceId = null);
        System.Collections.Generic.IEnumerable<NotificationViewModel> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
        void MarkAsRead(int notificationId);
        void MarkAllAsRead(int userId);
    }

    public interface IReviewService
    {
        void SubmitReview(int userId, SubmitReviewViewModel model);
        System.Collections.Generic.IEnumerable<ReviewViewModel> GetBusReviews(int busId);
        ReviewStatsViewModel GetBusReviewStats(int busId);
        void ApproveReview(int reviewId);
        void ReportAbuse(int reviewId, string reason);
        void DeleteReview(int reviewId);
    }

    public interface IReportService
    {
        DailyReportViewModel GetDailyReport(System.DateTime date);
        MonthlyReportViewModel GetMonthlyReport(int year, int month);
        YearlyReportViewModel GetYearlyReport(int year);
        BookingReportViewModel GetBookingReport(System.DateTime fromDate, System.DateTime toDate, string status = null);
        CancellationReportViewModel GetCancellationReport(System.DateTime fromDate, System.DateTime toDate);
        OperatorReportViewModel GetOperatorReport(int operatorId, System.DateTime fromDate, System.DateTime toDate);
        CustomerReportViewModel GetCustomerReport(int userId, System.DateTime fromDate, System.DateTime toDate);
        OccupancyReportViewModel GetOccupancyReport(int busId, System.DateTime fromDate, System.DateTime toDate);
    }

    public interface IBusService
    {
        System.Collections.Generic.IEnumerable<BusListViewModel> GetOperatorBuses(int operatorId);
        BusDetailViewModel GetBusDetail(int busId);
        void CreateBus(int operatorId, CreateBusViewModel model);
        void UpdateBus(EditBusViewModel model);
        void DeleteBus(int busId);
        void ToggleStatus(int busId);
        SeatLayoutViewModel GetSeatLayout(int busId);
        void UpdateSeatLayout(int busId, SeatLayoutViewModel model);
        void AddAmenity(int busId, int amenityId);
        void RemoveAmenity(int busId, int amenityId);
    }

    public interface IOperatorService
    {
        OperatorDashboardViewModel GetDashboard(int operatorId);
        void UpdateProfile(int operatorId, OperatorProfileViewModel model);
        OperatorProfileViewModel GetProfile(int operatorId);
        System.Collections.Generic.IEnumerable<ScheduleViewModel> GetSchedules(int operatorId);
        void CreateSchedule(int operatorId, CreateScheduleViewModel model);
        void UpdateSchedule(EditScheduleViewModel model);
        void DeleteSchedule(int scheduleId);
        System.Collections.Generic.IEnumerable<PricingRuleViewModel> GetPricingRules(int operatorId);
        void UpdatePricing(int operatorId, PricingViewModel model);
    }

    public interface IAdminService
    {
        AdminDashboardViewModel GetDashboard();
        System.Collections.Generic.IEnumerable<UserListViewModel> GetUsers(int page = 1, int pageSize = 20);
        System.Collections.Generic.IEnumerable<OperatorListViewModel> GetOperators(int page = 1, int pageSize = 20);
        System.Collections.Generic.IEnumerable<BusListViewModel> GetAllBuses(int page = 1, int pageSize = 20);
        void ApproveOperator(int operatorId);
        void SuspendUser(int userId);
        void ActivateUser(int userId);
        void AssignRole(int userId, string roleName);
        void RemoveRole(int userId, string roleName);
        System.Collections.Generic.IEnumerable<CouponViewModel> GetCoupons();
        void CreateCoupon(CreateCouponViewModel model);
        void ToggleCouponStatus(int couponId);
        System.Collections.Generic.IEnumerable<ReviewViewModel> GetPendingReviews();
        void ApproveReview(int reviewId);
        void RejectReview(int reviewId);
    }

    public interface IAuditService
    {
        void Log(string action, string entityType, int? entityId, int? userId,
                 string oldValues = null, string newValues = null);
        System.Collections.Generic.IEnumerable<AuditLogViewModel> GetLogs(int page = 1, int pageSize = 50);
        System.Collections.Generic.IEnumerable<AuditLogViewModel> GetLogsByUser(int userId, int page = 1, int pageSize = 50);
        System.Collections.Generic.IEnumerable<AuditLogViewModel> GetLogsByEntity(string entityType, int entityId);
    }
}
