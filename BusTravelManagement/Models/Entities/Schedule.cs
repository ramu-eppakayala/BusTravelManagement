using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BusId { get; set; }
        public int RouteId { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }

        [StringLength(20)]
        public string Frequency { get; set; } = "Daily";

        [StringLength(100)]
        public string DaysOfWeek { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal")]
        public decimal BaseFare { get; set; }

        [Column(TypeName = "decimal")]
        public decimal PerKmRate { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public bool IsRecurring { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }

        public virtual ICollection<ScheduleStop> ScheduleStops { get; set; }
        public virtual ICollection<BoardingPoint> BoardingPoints { get; set; }
        public virtual ICollection<DroppingPoint> DroppingPoints { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }

    [Table("ScheduleStops")]
    public class ScheduleStop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ScheduleId { get; set; }
        public int StopId { get; set; }

        public TimeSpan? ArrivalTime { get; set; }
        public TimeSpan? DepartureTime { get; set; }
        public int StopOrder { get; set; }

        [Column(TypeName = "decimal")]
        public decimal DistanceFromStart { get; set; } = 0;

        [Column(TypeName = "decimal")]
        public decimal FareMultiplier { get; set; } = 1.0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StopId")]
        public virtual Stop Stop { get; set; }
    }
}
