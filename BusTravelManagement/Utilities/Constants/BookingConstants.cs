namespace BusTravelManagement.Utilities.Constants
{
    public static class BookingConstants
    {
        // Booking Statuses
        public const string BookingStatusPending = "Pending";
        public const string BookingStatusConfirmed = "Confirmed";
        public const string BookingStatusCancelled = "Cancelled";
        public const string BookingStatusCompleted = "Completed";
        public const string BookingStatusRefunded = "Refunded";

        // Payment Statuses
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusSuccess = "Success";
        public const string PaymentStatusFailed = "Failed";
        public const string PaymentStatusRefunded = "Refunded";

        // Payment Gateways
        public const string GatewayMock = "Mock";
        public const string GatewayStripe = "Stripe";
        public const string GatewayRazorpay = "Razorpay";
        public const string GatewayPayPal = "PayPal";

        // Discount Types
        public const string DiscountPercentage = "Percentage";
        public const string DiscountFlat = "Flat";

        // Seat Statuses
        public const string SeatAvailable = "Available";
        public const string SeatBooked = "Booked";
        public const string SeatReserved = "Reserved";
        public const string SeateLadies = "Ladies";
        public const string SeatOnHold = "OnHold";

        // Seat Positions
        public const string SeatWindow = "Window";
        public const string SeatAisle = "Aisle";
        public const string SeatMiddle = "Middle";

        // Seat Layout Types
        public const string Layout2x2 = "2x2";
        public const string Layout2x1 = "2x1";
        public const string Layout1x2 = "1x2";
        public const string Layout3x2 = "3x2";
        public const string Layout2x3 = "2x3";
        public const string LayoutSleeper = "Sleeper";

        // Frequency
        public const string FrequencyDaily = "Daily";
        public const string FrequencyWeekly = "Weekly";
        public const string FrequencyCustom = "Custom";

        // Notification Types
        public const string NotificationEmail = "Email";
        public const string NotificationSMS = "SMS";
        public const string NotificationSystem = "System";

        // Refund Types
        public const string RefundFull = "Full";
        public const string RefundPartial = "Partial";

        // Transaction Types
        public const string TransactionCredit = "Credit";
        public const string TransactionDebit = "Debit";

        // Bus Seat Positions
        public const string DeckLower = "Lower";
        public const string DeckUpper = "Upper";
    }
}
