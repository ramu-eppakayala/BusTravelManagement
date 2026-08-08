using System;
using System.Security.Cryptography;
using System.Text;

namespace BusTravelManagement.Security
{
    public static class TokenHelper
    {
        public static string GenerateEmailVerificationToken()
        {
            return GenerateToken(64);
        }

        public static string GenerateResetPasswordToken()
        {
            return GenerateToken(48);
        }

        public static string GenerateApiToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-").Replace("=", "");
        }

        public static string GenerateBookingNumber()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = GenerateNumericString(6);
            return $"BT{date}{random}";
        }

        public static string GenerateTicketNumber()
        {
            var date = DateTime.UtcNow.ToString("yyMMdd");
            var random = GenerateAlphanumeric(8).ToUpper();
            return $"TKT{date}{random}";
        }

        public static string GeneratePaymentReference()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = GenerateNumericString(4);
            return $"PAY{date}{random}";
        }

        public static string GenerateRefundReference()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = GenerateNumericString(4);
            return $"RFN{date}{random}";
        }

        public static string GenerateQRData(string bookingNumber, int scheduleId, int busId, DateTime journeyDate)
        {
            var data = $"{bookingNumber}|{scheduleId}|{busId}|{journeyDate:yyyy-MM-dd}|{Guid.NewGuid():N}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        private static string GenerateToken(int length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Replace("=", "")
                    .Substring(0, Math.Min(length, 64));
            }
        }

        private static string GenerateNumericString(int length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                var result = new char[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = (char)('0' + (bytes[i] % 10));
                }
                return new string(result);
            }
        }

        private static string GenerateAlphanumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                var result = new char[length];
                for (int i = 0; i < length; i++)
                {
                    result[i] = chars[bytes[i] % chars.Length];
                }
                return new string(result);
            }
        }
    }
}
