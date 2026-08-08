namespace BusTravelManagement.Utilities.Constants
{
    public static class AppConstants
    {
        // Application
        public const string AppName = "BusTravel Management";
        public const string AppVersion = "1.0.0";
        public const string CompanyName = "BusTravel Inc.";
        public const string SupportEmail = "support@bustravel.local";
        public const string SupportPhone = "+1-800-BUS-TRIP";
        public const string WebsiteUrl = "https://bustravel.local";

        // Pagination
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        // Booking Limits
        public const int MaxSeatsPerBooking = 6;
        public const int BookingTimeoutMinutes = 15;
        public const int CancellationWindowHours = 24;
        public const int DefaultRefundPercentage = 75;
        public const int MinAgeForTravel = 1;
        public const int MaxAgeForTravel = 120;

        // Login Security
        public const int MaxFailedLoginAttempts = 5;
        public const int RememberMeDays = 14;
        public const int SessionTimeoutMinutes = 30;
        public const int ResetTokenExpiryHours = 24;

        // Fees & Taxes
        public const decimal ConvenienceFee = 20.00m;
        public const decimal TaxPercentage = 5.00m;
        public const decimal GSTPercentage = 5.00m;
        public const decimal DefaultCommissionPercentage = 10.00m;

        // Cache Keys
        public const string CacheCitiesKey = "Cities_All";
        public const string CachePopularCitiesKey = "Cities_Popular";
        public const string CacheOperatorsKey = "Operators_Active";
        public const string CacheBusTypesKey = "BusTypes_All";
        public const string CacheAmenitiesKey = "Amenities_All";

        // Cache Duration (minutes)
        public const int CacheShortDuration = 5;
        public const int CacheMediumDuration = 30;
        public const int CacheLongDuration = 120;

        // Session Keys
        public const string SessionBookingKey = "Booking_Current";
        public const string SessionSeatHoldKey = "Seat_Hold";
        public const string SessionTempDataKey = "TempData";

        // Cookie Names
        public const string CookieRecentSearches = "RecentSearches";
        public const string CookieUserPreferences = "UserPreferences";

        // Date Formats
        public const string DateFormat = "dd MMM yyyy";
        public const string TimeFormat = "hh:mm tt";
        public const string DateTimeFormat = "dd MMM yyyy hh:mm tt";
        public const string DateTimeFullFormat = "dddd, dd MMM yyyy hh:mm tt";

        // Currency
        public const string CurrencySymbol = "₹";
        public const string CurrencyCode = "INR";

        // File Paths
        public const string TicketPdfPath = "~/Content/tickets/";
        public const string ProfileImagePath = "~/Content/uploads/profiles/";
        public const string BusGalleryPath = "~/Content/uploads/buses/";
        public const string OperatorLogoPath = "~/Content/uploads/operators/";
        public const string DefaultProfileImage = "~/Content/images/default-avatar.png";
        public const string DefaultBusImage = "~/Content/images/default-bus.png";

        // QR Code
        public const int QRCodeSize = 200;
    }
}
