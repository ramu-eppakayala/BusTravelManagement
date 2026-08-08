using System;

namespace BusTravelManagement.Utilities.Helpers
{
    public class SmsHelper
    {
        private readonly bool _isEnabled;
        private readonly string _apiKey;
        private readonly string _senderId;

        public SmsHelper()
        {
            _isEnabled = bool.TryParse(
                System.Configuration.ConfigurationManager.AppSettings["app:EnableSms"], out var enabled) && enabled;
            _apiKey = System.Configuration.ConfigurationManager.AppSettings["app:SmsApiKey"] ?? "";
            _senderId = System.Configuration.ConfigurationManager.AppSettings["app:SmsSenderId"] ?? "BUSTRL";
        }

        public bool Send(string phoneNumber, string message)
        {
            if (!_isEnabled)
            {
                var log = log4net.LogManager.GetLogger(typeof(SmsHelper));
                log.Info($"SMS not sent (disabled): To={phoneNumber}, Message={message}");
                return false;
            }

            try
            {
                // In production, integrate with SMS gateway API here
                // e.g., Twilio, MSG91, Amazon SNS, etc.
                var log = log4net.LogManager.GetLogger(typeof(SmsHelper));
                log.Info($"SMS sent to {phoneNumber}: {message}");
                return true;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(SmsHelper));
                log.Error($"Failed to send SMS to {phoneNumber}", ex);
                return false;
            }
        }

        public bool SendBookingConfirmation(string phone, string bookingNumber, string busInfo)
        {
            var message = $"BusTravel: Booking {bookingNumber} confirmed for {busInfo}. Thank you!";
            return Send(phone, message);
        }
    }
}
