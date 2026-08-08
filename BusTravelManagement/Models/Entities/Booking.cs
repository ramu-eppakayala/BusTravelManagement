using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(20)]
        [Index(IsUnique = true)]
        public string BookingNumber { get; set; }

        public int UserId { get; set; }
        public int ScheduleId { get; set; }

        [Column(TypeName = "date")]
        public DateTime JourneyDate { get; set; }

        public int SourceStopId { get; set; }
        public int DestinationStopId { get; set; }
        public int BoardingPointId { get; set; }
        public int DroppingPointId { get; set; }

        [StringLength(20)]
        public string BookingStatus { get; set; } = "Pending";

        public int NumberOfSeats { get; set; }

        [Column(TypeName = "decimal")]
        public decimal TotalFare { get; set; }

        [Column(TypeName = "decimal")]
        public decimal ConvenienceFee { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal DiscountAmount { get; set; } = 0;

        public int? CouponId { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        [Column(TypeName = "decimal")]
        public decimal CouponDiscount { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal NetAmount { get; set; }

        [Column(TypeName = "decimal")]
        public decimal RefundAmount { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal CancellationCharge { get; set; } = 0;

        [StringLength(100)]
        public string ContactName { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        [StringLength(100)]
        public string ContactEmail { get; set; }

        [StringLength(500)]
        public string SpecialRequests { get; set; }

        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public int? CancelledBy { get; set; }

        [StringLength(500)]
        public string CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        public virtual ICollection<BookingPassenger> BookingPassengers { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        [ForeignKey("BoardingPointId")]
        public virtual BoardingPoint BoardingPoint { get; set; }

        [ForeignKey("DroppingPointId")]
        public virtual DroppingPoint DroppingPoint { get; set; }
    }

    [Table("BookingPassengers")]
    public class BookingPassenger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookingId { get; set; }
        public int? SeatId { get; set; }

        [Required, StringLength(100)]
        public string PassengerName { get; set; }

        public int Age { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public bool IsLadiesSeat { get; set; } = false;

        [StringLength(50)]
        public string IdCardType { get; set; }

        [StringLength(100)]
        public string IdCardNumber { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Fare { get; set; }

        [Column(TypeName = "decimal")]
        public decimal CancellationCharge { get; set; } = 0;

        public bool IsCancelled { get; set; } = false;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [ForeignKey("SeatId")]
        public virtual Seat Seat { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("Payments")]
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [Required, StringLength(100)]
        [Index(IsUnique = true)]
        public string PaymentReference { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(50)]
        public string PaymentGateway { get; set; } = "Mock";

        [StringLength(200)]
        public string GatewayTransactionId { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "INR";

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [StringLength(500)]
        public string FailureReason { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    [Table("Refunds")]
    public class Refund
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PaymentId { get; set; }
        public int BookingId { get; set; }

        [Required, StringLength(100)]
        public string RefundReference { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string RefundType { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [Required, StringLength(50)]
        [Index(IsUnique = true)]
        public string TicketNumber { get; set; }

        public string QRCodeData { get; set; }

        [StringLength(500)]
        public string PdfPath { get; set; }

        public string CancellationPolicy { get; set; }
        public DateTime? CancellationDeadline { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(20)]
        public string DiscountType { get; set; }

        [Column(TypeName = "decimal")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? MinBookingAmount { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        public int PerUserLimit { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    [Table("Reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }

        public int Rating { get; set; } // 1-5

        [StringLength(2000)]
        public string ReviewText { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsAbuseReported { get; set; } = false;

        [StringLength(500)]
        public string AbuseReportReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }
    }

    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Message { get; set; }

        [StringLength(10)]
        public string NotificationType { get; set; } = "System";

        [StringLength(50)]
        public string ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Action { get; set; }

        [StringLength(100)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }
        public int? UserId { get; set; }

        public string OldValues { get; set; }
        public string NewValues { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; }

        [StringLength(500)]
        public string UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Index]
        public DateTime? CreatedAtIndex { get; set; }
    }

    [Table("WalletTransactions")]
    public class WalletTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(10)]
        public string TransactionType { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal")]
        public decimal BalanceAfter { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ReferenceType { get; set; }

        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    [Table("CancellationPolicies")]
    public class CancellationPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OperatorId { get; set; }
        public int HoursBeforeDeparture { get; set; }

        [Column(TypeName = "decimal")]
        public decimal RefundPercentage { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OperatorId")]
        public virtual Operator Operator { get; set; }
    }
}
