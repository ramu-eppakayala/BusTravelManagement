using System;
using System.Collections.Generic;
using System.Text;

namespace BusTravelManagement.Utilities.Helpers
{
    public static class TicketHelper
    {
        public static string GenerateCancellationPolicyText(int cancellationWindowHours, decimal refundPercentage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("CANCELLATION POLICY");
            sb.AppendLine("===================");
            sb.AppendLine();
            sb.AppendLine($"1. Cancellations made {cancellationWindowHours} hours before departure: {refundPercentage}% refund.");
            sb.AppendLine($"2. Cancellations made within {cancellationWindowHours} hours of departure: No refund.");
            sb.AppendLine("3. Partial cancellation of seats is allowed.");
            sb.AppendLine("4. Cancellation charges are calculated per seat.");
            sb.AppendLine("5. Convenience fee is non-refundable.");
            sb.AppendLine("6. Refund will be processed within 5-7 business days.");
            return sb.ToString();
        }

        public static string FormatTicketHtml(
            string busNumber, string operatorName, string sourceCity, string destinationCity,
            DateTime journeyDate, string departureTime, string arrivalTime,
            string boardingPoint, string boardingTime, string droppingPoint, string droppingTime,
            string passengerName, List<string> seatNumbers, int numberOfSeats,
            decimal totalAmount, string bookingNumber, string ticketNumber,
            string contactPhone, string contactEmail)
        {
            var seats = string.Join(", ", seatNumbers);
            var dateStr = journeyDate.ToString("dddd, dd MMM yyyy");

            return $@"
            <html><head>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; }}
                .header {{ background: #2c3e50; color: white; padding: 15px; text-align: center; }}
                .content {{ padding: 20px; }}
                .details {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
                .details td {{ padding: 8px; border-bottom: 1px solid #ddd; }}
                .details tr:nth-child(even) {{ background: #f9f9f9; }}
                .label {{ font-weight: bold; color: #555; width: 150px; }}
                .footer {{ text-align: center; color: #888; font-size: 12px; margin-top: 30px; }}
                .barcode {{ text-align: center; margin: 20px 0; }}
                .status {{ background: #27ae60; color: white; padding: 5px 15px; display: inline-block; border-radius: 3px; }}
            </style></head><body>
                <div class='header'>
                    <h2>BusTravel - E-Ticket</h2>
                    <p>Booking: {bookingNumber} | Ticket: {ticketNumber}</p>
                    <span class='status'>CONFIRMED</span>
                </div>
                <div class='content'>
                    <table class='details'>
                        <tr><td class='label'>Bus</td><td>{busNumber} - {operatorName}</td></tr>
                        <tr><td class='label'>Route</td><td>{sourceCity} → {destinationCity}</td></tr>
                        <tr><td class='label'>Date</td><td>{dateStr}</td></tr>
                        <tr><td class='label'>Departure</td><td>{departureTime}</td></tr>
                        <tr><td class='label'>Arrival</td><td>{arrivalTime}</td></tr>
                        <tr><td class='label'>Boarding</td><td>{boardingPoint} at {boardingTime}</td></tr>
                        <tr><td class='label'>Dropping</td><td>{droppingPoint} at {droppingTime}</td></tr>
                        <tr><td class='label'>Passenger</td><td>{passengerName}</td></tr>
                        <tr><td class='label'>Seats</td><td>{seats} ({numberOfSeats} seat(s))</td></tr>
                        <tr><td class='label'>Total Paid</td><td>₹{totalAmount:N2}</td></tr>
                        <tr><td class='label'>Contact</td><td>{contactPhone} | {contactEmail}</td></tr>
                    </table>
                    <div class='barcode'>[QR Code Placeholder]</div>
                </div>
                <div class='footer'>
                    <p>This is a computer-generated ticket. No signature required.</p>
                    <p>Please carry a printout or digital copy of this ticket during travel.</p>
                    <p>Report issues: support@bustravel.local | +1-800-BUS-TRIP</p>
                </div>
            </body></html>";
        }
    }
}
