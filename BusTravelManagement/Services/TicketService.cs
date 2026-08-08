using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TicketViewModel GenerateTicket(string bookingNumber)
        {
            try
            {
                var booking = _unitOfWork.Query<Booking>()
                    .Include("Schedule")
                    .Include("Schedule.Bus")
                    .Include("Schedule.Bus.Operator")
                    .Include("Schedule.Route.SourceCity")
                    .Include("Schedule.Route.DestinationCity")
                    .Include("BoardingPoint")
                    .Include("DroppingPoint")
                    .Include("BookingPassengers")
                    .Include("User")
                    .FirstOrDefault(b => b.BookingNumber == bookingNumber);

                if (booking == null) return null;

                // Check if ticket already exists
                var existingTicket = _unitOfWork.Tickets.GetByBookingId(booking.Id);
                if (existingTicket != null)
                {
                    return MapTicketViewModel(existingTicket, booking);
                }

                var schedule = booking.Schedule;
                var bus = schedule.Bus;

                // Generate unique ticket number
                var ticketNumber = GenerateTicketNumber();

                // Build QR code data (simulated - base64 encoded booking info)
                var qrData = BuildQRCodeData(booking, ticketNumber);
                var qrCodeBase64 = GenerateQRCode(qrData);

                // Determine cancellation policy
                var policy = _unitOfWork.Query<CancellationPolicy>()
                    .Where(cp => cp.OperatorId == bus.OperatorId && cp.IsActive)
                    .OrderBy(cp => cp.HoursBeforeDeparture)
                    .ToList();

                var cancellationPolicyText = policy.Any()
                    ? string.Join(" | ", policy.Select(p =>
                        $"Cancel {p.HoursBeforeDeparture}+ hrs before: {p.RefundPercentage}% refund"))
                    : "Standard cancellation policy applies.";

                var cancellationDeadline = booking.JourneyDate
                    .Add(schedule.DepartureTime)
                    .AddHours(-1);

                // Create ticket record
                var ticket = new Ticket
                {
                    BookingId = booking.Id,
                    TicketNumber = ticketNumber,
                    QRCodeData = qrCodeBase64,
                    CancellationPolicy = cancellationPolicyText,
                    CancellationDeadline = cancellationDeadline,
                    GeneratedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Tickets.Add(ticket);
                _unitOfWork.SaveChanges();

                return MapTicketViewModel(ticket, booking);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Error("GenerateTicket error", ex);
                return null;
            }
        }

        public TicketViewModel GetTicket(string bookingNumber)
        {
            try
            {
                var booking = _unitOfWork.Query<Booking>()
                    .Include("Schedule")
                    .Include("Schedule.Bus")
                    .Include("Schedule.Bus.Operator")
                    .Include("Schedule.Route.SourceCity")
                    .Include("Schedule.Route.DestinationCity")
                    .Include("BoardingPoint")
                    .Include("DroppingPoint")
                    .Include("BookingPassengers")
                    .Include("Tickets")
                    .Include("User")
                    .FirstOrDefault(b => b.BookingNumber == bookingNumber);

                if (booking == null) return null;

                var ticket = booking.Tickets?.FirstOrDefault();
                if (ticket == null) return null;

                return MapTicketViewModel(ticket, booking);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Error("GetTicket error", ex);
                return null;
            }
        }

        public string GenerateQRCode(string data)
        {
            try
            {
                // Simulate QR code generation by returning a base64-encoded string
                var bytes = Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Error("GenerateQRCode error", ex);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes("QR_GENERATION_FAILED"));
            }
        }

        public byte[] GenerateTicketPdf(string bookingNumber)
        {
            try
            {
                // Placeholder for PDF generation
                // In a real implementation, this would use a PDF library like iTextSharp
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Info($"PDF generation requested for booking: {bookingNumber}");

                // Return empty byte array as placeholder
                return new byte[0];
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Error("GenerateTicketPdf error", ex);
                return new byte[0];
            }
        }

        public void SendTicketEmail(string bookingNumber)
        {
            try
            {
                var ticket = GetTicket(bookingNumber);
                if (ticket == null) return;

                var emailBody = BuildTicketEmailBody(ticket);
                var notificationService = new NotificationService(_unitOfWork);
                notificationService.SendEmail(ticket.ContactEmail, $"BusTravel Ticket - {ticket.TicketNumber}", emailBody);
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(TicketService));
                log.Error("SendTicketEmail error", ex);
            }
        }

        private string GenerateTicketNumber()
        {
            var prefix = "TKT";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999).ToString();
            return $"{prefix}{timestamp}{random}";
        }

        private string BuildQRCodeData(Booking booking, string ticketNumber)
        {
            var schedule = booking.Schedule;
            var bus = schedule.Bus;

            return string.Join("|",
                "BUSTRAVEL",
                ticketNumber,
                booking.BookingNumber,
                bus.BusNumber,
                bus.Operator.CompanyName,
                schedule.Route.SourceCity.Name,
                schedule.Route.DestinationCity.Name,
                booking.JourneyDate.ToString("yyyy-MM-dd"),
                schedule.DepartureTime.ToString(@"hh\:mm"),
                schedule.ArrivalTime.ToString(@"hh\:mm"),
                booking.BoardingPoint?.Name ?? "",
                booking.DroppingPoint?.Name ?? "",
                booking.BookingPassengers?.Count.ToString() ?? "0",
                booking.NetAmount.ToString("F2")
            );
        }

        private string BuildTicketEmailBody(TicketViewModel ticket)
        {
            var sb = new StringBuilder();
            sb.Append("<h2>Your BusTravel Ticket</h2>");
            sb.Append($"<p><strong>Ticket Number:</strong> {ticket.TicketNumber}</p>");
            sb.Append($"<p><strong>Bus:</strong> {ticket.BusNumber} ({ticket.BusType})</p>");
            sb.Append($"<p><strong>Operator:</strong> {ticket.OperatorName}</p>");
            sb.Append($"<p><strong>Route:</strong> {ticket.SourceCity} - {ticket.DestinationCity}</p>");
            sb.Append($"<p><strong>Journey Date:</strong> {ticket.JourneyDate:dd MMM yyyy}</p>");
            sb.Append($"<p><strong>Departure:</strong> {ticket.DepartureTime}</p>");
            sb.Append($"<p><strong>Arrival:</strong> {ticket.ArrivalTime}</p>");
            sb.Append($"<p><strong>Boarding Point:</strong> {ticket.BoardingPoint} at {ticket.BoardingTime}</p>");
            sb.Append($"<p><strong>Dropping Point:</strong> {ticket.DroppingPoint} at {ticket.DroppingTime}</p>");
            sb.Append($"<p><strong>Passengers:</strong> {ticket.NumberOfSeats}</p>");
            sb.Append($"<p><strong>Amount Paid:</strong> {ticket.NetAmount:C}</p>");
            sb.Append("<p>Thank you for choosing BusTravel!</p>");
            return sb.ToString();
        }

        private TicketViewModel MapTicketViewModel(Ticket ticket, Booking booking)
        {
            var schedule = booking.Schedule;
            var bus = schedule.Bus;

            return new TicketViewModel
            {
                TicketNumber = ticket.TicketNumber,
                BookingNumber = booking.BookingNumber,
                BusNumber = bus.BusNumber,
                OperatorName = bus.Operator.CompanyName,
                OperatorLogo = bus.Operator.LogoUrl,
                BusType = bus.BusType?.Name ?? "Standard",
                IsAC = bus.IsAC,
                SourceCity = schedule.Route.SourceCity.Name,
                DestinationCity = schedule.Route.DestinationCity.Name,
                JourneyDate = booking.JourneyDate,
                DepartureTime = schedule.DepartureTime.ToString(@"hh\:mm"),
                ArrivalTime = schedule.ArrivalTime.ToString(@"hh\:mm"),
                BoardingPoint = booking.BoardingPoint?.Name,
                BoardingTime = booking.BoardingPoint?.PickupTime.ToString(@"hh\:mm"),
                DroppingPoint = booking.DroppingPoint?.Name,
                DroppingTime = booking.DroppingPoint?.DropTime.ToString(@"hh\:mm"),
                NumberOfSeats = booking.NumberOfSeats,
                SeatNumbers = booking.BookingPassengers
                    .Select(bp => bp.Seat?.SeatNumber ?? "N/A")
                    .ToList(),
                Passengers = booking.BookingPassengers.Select(bp => new PassengerDetailViewModel
                {
                    Name = bp.PassengerName,
                    Age = bp.Age,
                    Gender = bp.Gender,
                    SeatNumber = bp.Seat?.SeatNumber,
                    IdCardType = bp.IdCardType,
                    IdCardNumber = bp.IdCardNumber,
                    IsLadiesSeat = bp.IsLadiesSeat
                }).ToList(),
                NetAmount = booking.NetAmount,
                ContactPhone = booking.ContactPhone,
                ContactEmail = booking.ContactEmail,
                BookedAt = booking.CreatedAt,
                QRCodeBase64 = ticket.QRCodeData,
                CancellationPolicy = ticket.CancellationPolicy,
                CancellationDeadline = ticket.CancellationDeadline
            };
        }
    }
}
