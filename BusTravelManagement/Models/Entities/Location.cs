using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string Country { get; set; } = "India";

        [NotMapped]
        public string DisplayName => $"{Name}{(string.IsNullOrEmpty(State) ? "" : $", {State}")}";

        [Column(TypeName = "decimal")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsPopular { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Route> SourceRoutes { get; set; }
        public virtual ICollection<Route> DestinationRoutes { get; set; }
        public virtual ICollection<Stop> Stops { get; set; }
    }

    [Table("Routes")]
    public class Route
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Distance { get; set; }

        public int? DurationMinutes { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("SourceCityId")]
        public virtual City SourceCity { get; set; }

        [ForeignKey("DestinationCityId")]
        public virtual City DestinationCity { get; set; }

        public virtual ICollection<Stop> Stops { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }

    [Table("Stops")]
    public class Stop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RouteId { get; set; }
        public int? CityId { get; set; }

        [StringLength(200)]
        public string StopName { get; set; }

        public int StopOrder { get; set; }

        public TimeSpan? StopTime { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public virtual ICollection<ScheduleStop> ScheduleStops { get; set; }
    }

    [Table("SeatLayouts")]
    public class SeatLayout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BusId { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }

        [Required, StringLength(10)]
        public string SeatNumber { get; set; }

        [StringLength(10)]
        public string SeatPosition { get; set; } = "Aisle";

        [StringLength(10)]
        public string Deck { get; set; } = "Lower";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }
    }

    [Table("BoardingPoints")]
    public class BoardingPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ScheduleId { get; set; }
        public int? StopId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Landmark { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Longitude { get; set; }

        public TimeSpan PickupTime { get; set; }
        public int PickupDayOffset { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StopId")]
        public virtual Stop Stop { get; set; }
    }

    [Table("DroppingPoints")]
    public class DroppingPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ScheduleId { get; set; }
        public int? StopId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Landmark { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? Longitude { get; set; }

        public TimeSpan DropTime { get; set; }
        public int DropDayOffset { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StopId")]
        public virtual Stop Stop { get; set; }
    }

    [Table("Seats")]
    public class Seat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SeatLayoutId { get; set; }
        public int? ScheduleId { get; set; }
        public int? BookingId { get; set; }

        [Required, StringLength(10)]
        public string SeatNumber { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Available";

        public DateTime? JourneyDate { get; set; }

        public bool IsLadiesSeat { get; set; } = false;
        public bool IsOnHold { get; set; } = false;
        public DateTime? HoldExpiry { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Fare { get; set; }

        [ForeignKey("SeatLayoutId")]
        public virtual SeatLayout SeatLayout { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}
