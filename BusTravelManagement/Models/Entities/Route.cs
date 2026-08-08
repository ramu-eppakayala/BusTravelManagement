using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    public class City : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string Country { get; set; } = "India";

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPopular { get; set; } = false;

        [NotMapped]
        public string DisplayName => $"{Name}{(string.IsNullOrEmpty(State) ? "" : $", {State}")}";

        // Navigation
        public virtual ICollection<Route> SourceRoutes { get; set; }
        public virtual ICollection<Route> DestinationRoutes { get; set; }
        public virtual ICollection<Stop> Stops { get; set; }
    }

    public class Route : BaseEntity
    {
        public int SourceCityId { get; set; }
        public int DestinationCityId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Distance { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("SourceCityId")]
        public virtual City SourceCity { get; set; }

        [ForeignKey("DestinationCityId")]
        public virtual City DestinationCity { get; set; }

        // Navigation
        public virtual ICollection<Stop> Stops { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }

    public class Stop : BaseEntity
    {
        public int RouteId { get; set; }
        public int? CityId { get; set; }

        [Required]
        [StringLength(200)]
        public string StopName { get; set; }

        public int StopOrder { get; set; }

        public int StopTime { get; set; } // Minutes from start

        [StringLength(500)]
        public string Address { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("RouteId")]
        public virtual Route Route { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }
    }

    public class BoardingPoint : BaseEntity
    {
        public int ScheduleId { get; set; }
        public int? StopId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Landmark { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        public System.TimeSpan PickupTime { get; set; }

        public int PickupDayOffset { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StopId")]
        public virtual Stop Stop { get; set; }
    }

    public class DroppingPoint : BaseEntity
    {
        public int ScheduleId { get; set; }
        public int? StopId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Landmark { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        public System.TimeSpan DropTime { get; set; }

        public int DropDayOffset { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [ForeignKey("StopId")]
        public virtual Stop Stop { get; set; }
    }
}
