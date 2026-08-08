using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    public class SeatLayout : BaseEntity
    {
        public int BusId { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatPosition { get; set; } // Window, Aisle, Middle

        [StringLength(10)]
        public string Deck { get; set; } = "Lower"; // Lower, Upper

        public bool IsActive { get; set; } = true;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }

        public virtual ICollection<Seat> Seats { get; set; }
    }

    public class Seat : BaseEntity
    {
        public int SeatLayoutId { get; set; }
        public int? ScheduleId { get; set; }
        public int? BookingId { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Available"; // Available, Booked, Reserved, Ladies

        public System.DateTime? JourneyDate { get; set; }

        // For ladies seat indicator
        public bool IsLadiesSeat { get; set; } = false;

        // For blocking temporary holds
        public bool IsOnHold { get; set; } = false;
        public System.DateTime? HoldExpiry { get; set; }

        public decimal Fare { get; set; }

        [ForeignKey("SeatLayoutId")]
        public virtual SeatLayout SeatLayout { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}
