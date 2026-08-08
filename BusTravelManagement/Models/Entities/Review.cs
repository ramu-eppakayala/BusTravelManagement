using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    public class Review : BaseEntity
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string ReviewText { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsAbuseReported { get; set; } = false;

        public string AbuseReportReason { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }
    }

    public class Notification : BaseEntity
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [StringLength(20)]
        public string NotificationType { get; set; } // Email, SMS, System

        [StringLength(50)]
        public string ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;

        public System.DateTime? SentAt { get; set; }

        public System.DateTime? ReadAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    public class AuditLog : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [Required]
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
    }
}
