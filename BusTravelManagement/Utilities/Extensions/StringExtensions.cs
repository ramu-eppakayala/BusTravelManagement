using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BusTravelManagement.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
        }

        public static string ToSlug(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            var str = value.ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim('-');
            str = Regex.Replace(str, @"-+", "-");
            return str;
        }

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return email;
            var parts = email.Split('@');
            if (parts.Length != 2) return email;
            var name = parts[0];
            if (name.Length <= 2) return $"{name[0]}***@{parts[1]}";
            return $"{name.Substring(0, 2)}{new string('*', name.Length - 2)}@{parts[1]}";
        }

        public static string MaskPhone(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 6) return phone;
            return phone.Substring(0, 2) + new string('*', phone.Length - 4) + phone.Substring(phone.Length - 2);
        }

        public static string ToBase64(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        public static string FromBase64(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static string RemoveExtraSpaces(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return Regex.Replace(value.Trim(), @"\s+", " ");
        }
    }
}
