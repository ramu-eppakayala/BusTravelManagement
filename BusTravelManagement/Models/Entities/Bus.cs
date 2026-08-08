using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    public class Bus : BaseEntity
    {
        public int OperatorId { get; set; }
        public int? BusTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        public int TotalSeats { get; set; }

        [Required]
        [StringLength(20)]
        public string SeatLayoutType { get; set; } // 2x2, 2x1, 1x2, 3x2, 2x3, Sleeper

        public bool IsAC { get; set; } = false;

        public bool IsSleeper { get; set; } = false;

        public bool IsSingleAxle { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        [ForeignKey("OperatorId")]
        public virtual Operator Operator { get; set; }

        [ForeignKey("BusTypeId")]
        public virtual BusType BusType { get; set; }

        public virtual ICollection<BusAmenity> BusAmenities { get; set; }
        public virtual ICollection<SeatLayout> SeatLayouts { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<BusGallery> BusGalleries { get; set; }
    }

    public class Amenity : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string IconClass { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<BusAmenity> BusAmenities { get; set; }
    }

    public class BusAmenity : BaseEntity
    {
        public int BusId { get; set; }
        public int AmenityId { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }

        [ForeignKey("AmenityId")]
        public virtual Amenity Amenity { get; set; }
    }

    public class BusGallery : BaseEntity
    {
        public int BusId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string Caption { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }
    }
}
