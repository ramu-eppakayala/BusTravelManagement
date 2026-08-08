using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using BusTravelManagement.Models.Entities;

namespace BusTravelManagement.Models
{
    public class BusTravelDbContext : DbContext
    {
        public BusTravelDbContext() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
        }

        // Users & Auth
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Operators & Buses
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusType> BusTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<BusAmenity> BusAmenities { get; set; }
        public DbSet<BusGallery> BusGalleries { get; set; }

        // Locations
        public DbSet<City> Cities { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<SeatLayout> SeatLayouts { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<BoardingPoint> BoardingPoints { get; set; }
        public DbSet<DroppingPoint> DroppingPoints { get; set; }

        // Schedules
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleStop> ScheduleStops { get; set; }

        // Bookings
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingPassenger> BookingPassengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        // Marketing
        public DbSet<Coupon> Coupons { get; set; }

        // Reviews
        public DbSet<Review> Reviews { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }

        // Audit
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Wallet
        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        // Policies
        public DbSet<CancellationPolicy> CancellationPolicies { get; set; }

        // Saved
        public DbSet<SavedPassenger> SavedPassengers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // UserRole composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // UserRole relationships
            modelBuilder.Entity<UserRole>()
                .HasRequired(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasRequired(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Booking relationships
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId);

            // Review relationships
            modelBuilder.Entity<Review>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Bus)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BusId);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Booking)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookingId);

            // Bus relationships
            modelBuilder.Entity<Bus>()
                .HasRequired(b => b.Operator)
                .WithMany(o => o.Buses)
                .HasForeignKey(b => b.OperatorId);

            modelBuilder.Entity<Bus>()
                .HasOptional(b => b.BusType)
                .WithMany(bt => bt.Buses)
                .HasForeignKey(b => b.BusTypeId);

            // Scheule relationships
            modelBuilder.Entity<Schedule>()
                .HasRequired(s => s.Bus)
                .WithMany(b => b.Schedules)
                .HasForeignKey(s => s.BusId);

            modelBuilder.Entity<Schedule>()
                .HasRequired(s => s.Route)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RouteId);

            // Indexes
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100);

            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingNumber)
                .HasMaxLength(20);

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.CreatedAt)
                .HasColumnAnnotation("Index", new IndexAnnotation(
                    new System.ComponentModel.DataAnnotations.Schema.IndexAttribute("IX_CreatedAt")));

            // Decimal precision
            modelBuilder.Entity<Booking>().Property(b => b.TotalFare).HasPrecision(10, 2);
            modelBuilder.Entity<Booking>().Property(b => b.NetAmount).HasPrecision(10, 2);
            modelBuilder.Entity<Booking>().Property(b => b.ConvenienceFee).HasPrecision(10, 2);
            modelBuilder.Entity<Booking>().Property(b => b.TaxAmount).HasPrecision(10, 2);
            modelBuilder.Entity<Booking>().Property(b => b.DiscountAmount).HasPrecision(10, 2);
            modelBuilder.Entity<Booking>().Property(b => b.RefundAmount).HasPrecision(10, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(10, 2);
            modelBuilder.Entity<Coupon>().Property(c => c.DiscountValue).HasPrecision(10, 2);

            // String defaults
            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("nvarchar"));

            // Set default schema
            modelBuilder.HasDefaultSchema("dbo");
        }
    }
}
