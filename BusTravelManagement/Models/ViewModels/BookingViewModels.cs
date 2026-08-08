using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTravelManagement.Models.ViewModels
{
    public class BookingRequestViewModel
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public System.DateTime JourneyDate { get; set; }

        [Required(ErrorMessage = "Please select boarding point")]
        public int BoardingPointId { get; set; }

        [Required(ErrorMessage = "Please select dropping point")]
        public int DroppingPointId { get; set; }

        [Required(ErrorMessage = "Please select source stop")]
        public int SourceStopId { get; set; }

        [Required(ErrorMessage = "Please select destination stop")]
        public int DestinationStopId { get; set; }

        [Required(ErrorMessage = "Please select at least one seat")]
        [MinLength(1, ErrorMessage = "Please select at least one seat")]
        public List<int> SeatLayoutIds { get; set; }
    }

    public class PassengerDetailViewModel
    {
        [Required(ErrorMessage = "Passenger name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^[a-zA-Z\s\.]+$", ErrorMessage = "Name can only contain letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        [Display(Name = "ID Card Type")]
        public string IdCardType { get; set; }

        [StringLength(100)]
        [Display(Name = "ID Card Number")]
        public string IdCardNumber { get; set; }

        public string SeatNumber { get; set; }
        public int SeatLayoutId { get; set; }
        public bool IsLadiesSeat { get; set; }
    }

    public class ConfirmBookingViewModel
    {
        public int ScheduleId { get; set; }
        public string BookingNumber { get; set; }

        [Required(ErrorMessage = "Contact name is required")]
        [StringLength(100)]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Contact phone is required")]
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        [RegularExpression(@"^[0-9+\-\s()]{10,20}$", ErrorMessage = "Invalid phone number")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Display(Name = "Special Requests")]
        [StringLength(500)]
        public string SpecialRequests { get; set; }

        public List<PassengerDetailViewModel> Passengers { get; set; }

        [Display(Name = "Coupon Code")]
        [StringLength(50)]
        public string CouponCode { get; set; }

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; }

        public bool AcceptTerms { get; set; }

        public List<SeatViewModel> SelectedSeats { get; set; }
        public string OperatorName { get; set; }
        public string BusNumber { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public decimal BaseFare { get; set; }
        public decimal ConvenienceFee { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string BoardingPointName { get; set; }
        public string BoardingPointTime { get; set; }
    }

    public class BookingResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string BookingNumber { get; set; }
        public string PaymentReference { get; set; }
        public decimal Amount { get; set; }
        public int? BookingId { get; set; }
        public List<string> Errors { get; set; }
    }

    public class BookingConfirmationViewModel
    {
        public string BookingNumber { get; set; }
        public string TicketNumber { get; set; }
        public string BookingStatus { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Duration { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public string BoardingPoint { get; set; }
        public string BoardingTime { get; set; }
        public string DroppingPoint { get; set; }
        public string DroppingTime { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public int NumberOfSeats { get; set; }
        public List<string> SeatNumbers { get; set; }
        public decimal TotalFare { get; set; }
        public decimal ConvenienceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public System.DateTime BookedAt { get; set; }
        public string QRCodeBase64 { get; set; }
        public bool IsCancellable { get; set; }
        public System.DateTime? CancellationDeadline { get; set; }
    }

    public class BookingHistoryViewModel
    {
        public List<BookingHistoryItemViewModel> Bookings { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class BookingHistoryItemViewModel
    {
        public string BookingNumber { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string BookingStatus { get; set; }
        public decimal NetAmount { get; set; }
        public int NumberOfSeats { get; set; }
        public List<string> SeatNumbers { get; set; }
        public System.DateTime BookedAt { get; set; }
        public bool IsCancellable { get; set; }
    }

    public class BookingDetailViewModel
    {
        public string BookingNumber { get; set; }
        public string TicketNumber { get; set; }
        public string BookingStatus { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public string OperatorLogo { get; set; }
        public string BusType { get; set; }
        public bool IsAC { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public string BoardingPoint { get; set; }
        public string BoardingTime { get; set; }
        public string DroppingPoint { get; set; }
        public string DroppingTime { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public int NumberOfSeats { get; set; }
        public List<string> SeatNumbers { get; set; }
        public List<PassengerDetailViewModel> Passengers { get; set; }
        public decimal TotalFare { get; set; }
        public decimal ConvenienceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationCharge { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string CouponCode { get; set; }
        public System.DateTime BookedAt { get; set; }
        public bool IsCancelled { get; set; }
        public System.DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; }
        public bool IsCancellable { get; set; }
        public System.DateTime? CancellationDeadline { get; set; }
        public string QRCodeBase64 { get; set; }
        public List<StopViewModel> RouteStops { get; set; }
        public List<AmenityViewModel> Amenities { get; set; }
    }

    public class CancelBookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationCharge { get; set; }
    }

    public class PaymentRequestViewModel
    {
        [Required]
        public string BookingNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentGateway { get; set; } = "Mock";
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }
        public string CardHolderName { get; set; }
        public string OperatorName { get; set; }
        public string BusNumber { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public System.DateTime TravelDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public List<string> SelectedSeats { get; set; }
    }

    public class PaymentResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PaymentReference { get; set; }
        public string GatewayTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string BookingNumber { get; set; }
    }

    public class PaymentStatusViewModel
    {
        public string PaymentReference { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime? PaidAt { get; set; }
        public string FailureReason { get; set; }
    }

    public class RefundResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RefundReference { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundType { get; set; }
    }

    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class TicketViewModel
    {
        public string TicketNumber { get; set; }
        public string BookingNumber { get; set; }
        public string BusNumber { get; set; }
        public string OperatorName { get; set; }
        public string OperatorLogo { get; set; }
        public string BusType { get; set; }
        public bool IsAC { get; set; }
        public string SourceCity { get; set; }
        public string DestinationCity { get; set; }
        public System.DateTime JourneyDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string BoardingPoint { get; set; }
        public string BoardingTime { get; set; }
        public string DroppingPoint { get; set; }
        public string DroppingTime { get; set; }
        public string PassengerName { get; set; }
        public int NumberOfSeats { get; set; }
        public List<string> SeatNumbers { get; set; }
        public List<PassengerDetailViewModel> Passengers { get; set; }
        public decimal NetAmount { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public System.DateTime BookedAt { get; set; }
        public string QRCodeBase64 { get; set; }
        public string CancellationPolicy { get; set; }
        public System.DateTime? CancellationDeadline { get; set; }
    }
}
