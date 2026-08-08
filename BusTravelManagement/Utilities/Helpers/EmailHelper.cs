using System;
using System.Net;
using System.Net.Mail;

namespace BusTravelManagement.Utilities.Helpers
{
    public class EmailHelper
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromAddress;
        private readonly string _fromName;

        public EmailHelper()
        {
            _smtpHost = System.Configuration.ConfigurationManager.AppSettings["app:SmtpHost"] ?? "localhost";
            _smtpPort = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["app:SmtpPort"], out var port) ? port : 587;
            _enableSsl = bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["app:SmtpEnableSsl"], out var ssl) && ssl;
            _userName = System.Configuration.ConfigurationManager.AppSettings["app:SmtpUserName"] ?? "";
            _password = System.Configuration.ConfigurationManager.AppSettings["app:SmtpPassword"] ?? "";
            _fromAddress = System.Configuration.ConfigurationManager.AppSettings["app:EmailFromAddress"] ?? "noreply@bustravel.local";
            _fromName = System.Configuration.ConfigurationManager.AppSettings["app:EmailFromName"] ?? "BusTravel Support";
        }

        public bool Send(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.EnableSsl = _enableSsl;
                    client.UseDefaultCredentials = false;

                    if (!string.IsNullOrEmpty(_userName))
                    {
                        client.Credentials = new NetworkCredential(_userName, _password);
                    }

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(_fromAddress, _fromName);
                        message.To.Add(new MailAddress(to));
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;

                        client.Send(message);
                    }
                }

                var log = log4net.LogManager.GetLogger(typeof(EmailHelper));
                log.Info($"Email sent to {to}: {subject}");
                return true;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(EmailHelper));
                log.Error($"Failed to send email to {to}: {subject}", ex);
                return false;
            }
        }

        public bool SendBookingConfirmation(string to, string userName, string bookingNumber, string ticketUrl)
        {
            var subject = $"Booking Confirmed - {bookingNumber}";
            var body = $@"
                <html><body>
                <h2>Booking Confirmed!</h2>
                <p>Dear {userName},</p>
                <p>Your booking <strong>{bookingNumber}</strong> has been confirmed.</p>
                <p><a href='{ticketUrl}'>View your ticket</a></p>
                <p>Thank you for choosing BusTravel!</p>
                </body></html>";
            return Send(to, subject, body);
        }

        public bool SendCancellationConfirmation(string to, string userName, string bookingNumber, decimal refundAmount)
        {
            var subject = $"Booking Cancelled - {bookingNumber}";
            var body = $@"
                <html><body>
                <h2>Booking Cancelled</h2>
                <p>Dear {userName},</p>
                <p>Your booking <strong>{bookingNumber}</strong> has been cancelled.</p>
                <p>Refund Amount: ₹{refundAmount:N2}</p>
                <p>Thank you for choosing BusTravel!</p>
                </body></html>";
            return Send(to, subject, body);
        }
    }
}
