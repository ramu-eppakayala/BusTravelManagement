using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    public class Payment : BaseEntity
    {
        public int BookingId { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string PaymentReference { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentGateway { get; set; } // Mock, Stripe, Razorpay, PayPal

        [StringLength(200)]
        public string GatewayTransactionId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "INR";

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } // Pending, Success, Failed, Refunded

        public string FailureReason { get; set; }

        public System.DateTime? PaidAt { get; set; }

        public System.DateTime? RefundedAt { get; set; }

        // Navigation
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        public virtual ICollection<Refund> Refunds { get; set; }
    }

    public class Refund : BaseEntity
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string RefundReference { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string RefundType { get; set; } // Full, Partial

        public string Reason { get; set; }

        public System.DateTime? ProcessedAt { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    public class Ticket : BaseEntity
    {
        public int BookingId { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string TicketNumber { get; set; }

        public string QRCodeData { get; set; }

        [StringLength(500)]
        public string PdfPath { get; set; }

        public string CancellationPolicy { get; set; }

        public System.DateTime? CancellationDeadline { get; set; }

        public System.DateTime GeneratedAt { get; set; } = System.DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }

    public class Coupon : BaseEntity
    {
        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(15)]
        public string DiscountType { get; set; } // Percentage, Flat

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinBookingAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; } = 0;

        public int PerUserLimit { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public System.DateTime ValidFrom { get; set; }

        public System.DateTime ValidTo { get; set; }

        // Navigation
        public virtual ICollection<Booking> Bookings { get; set; }
    }

    public class WalletTransaction : BaseEntity
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string TransactionType { get; set; } // Credit, Debit

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceAfter { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
