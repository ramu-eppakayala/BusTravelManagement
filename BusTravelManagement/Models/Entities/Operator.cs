using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusTravelManagement.Models.Entities
{
    [Table("Operators")]
    public class Operator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required, StringLength(200)]
        public string CompanyName { get; set; }

        [StringLength(100)]
        public string CompanyRegistrationNumber { get; set; }

        [Required, StringLength(100)]
        public string ContactPerson { get; set; }

        [StringLength(100)]
        public string ContactEmail { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(200)]
        public string WebsiteUrl { get; set; }

        [StringLength(500)]
        public string LogoUrl { get; set; }

        [Column(TypeName = "decimal")]
        public decimal CommissionPercentage { get; set; } = 10.00m;

        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<Bus> Buses { get; set; }
        public virtual ICollection<CancellationPolicy> CancellationPolicies { get; set; }
    }

    [Table("BusTypes")]
    public class BusType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Bus> Buses { get; set; }
    }

    [Table("Buses")]
    public class Bus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OperatorId { get; set; }
        public int? BusTypeId { get; set; }

        [Required, StringLength(50)]
        public string BusNumber { get; set; }

        [Required, StringLength(50)]
        public string RegistrationNumber { get; set; }

        public int TotalSeats { get; set; }

        [StringLength(20)]
        public string SeatLayoutType { get; set; } = "2x2";

        public bool IsAC { get; set; } = false;
        public bool IsSleeper { get; set; } = false;
        public bool IsSingleAxle { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

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

    [Table("Amenities")]
    public class Amenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string IconClass { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<BusAmenity> BusAmenities { get; set; }
    }

    [Table("BusAmenities")]
    public class BusAmenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BusId { get; set; }
        public int AmenityId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }

        [ForeignKey("AmenityId")]
        public virtual Amenity Amenity { get; set; }
    }

    [Table("BusGalleries")]
    public class BusGallery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int BusId { get; set; }

        [Required, StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        public string Caption { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BusId")]
        public virtual Bus Bus { get; set; }
    }
}
