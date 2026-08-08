using System;

namespace BusTravelManagement.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var now = DateTime.UtcNow;
            var span = now - dateTime;

            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
            if (span.TotalDays < 365) return $"{(int)(span.TotalDays / 30)}mo ago";
            return $"{(int)(span.TotalDays / 365)}y ago";
        }

        public static string ToDisplayDate(this DateTime dateTime, string format = "dd MMM yyyy")
        {
            return dateTime.ToString(format);
        }

        public static string ToDisplayTime(this DateTime dateTime, string format = "hh:mm tt")
        {
            return dateTime.ToString(format);
        }

        public static string ToDisplayDateTime(this DateTime dateTime, string format = "dd MMM yyyy hh:mm tt")
        {
            return dateTime.ToString(format);
        }

        public static DateTime ToStartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, DateTimeKind.Utc);
        }

        public static DateTime ToStartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime ToEndOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month,
                DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59, DateTimeKind.Utc);
        }

        public static DateTime ToStartOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime ToEndOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static string TimeSpanToString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
            return $"{timeSpan.Minutes}m";
        }

        public static TimeSpan ToTimeSpan(this string timeString)
        {
            if (TimeSpan.TryParse(timeString, out var result))
                return result;
            return TimeSpan.Zero;
        }
    }
}
