using System;
using System.IO;
using System.Web;

namespace BusTravelManagement.Utilities.Helpers
{
    public static class PdfHelper
    {
        public static byte[] GenerateTicketPdf(string htmlContent)
        {
            try
            {
                // In production, use a library like iTextSharp, IronPDF, or SelectPDF
                // For development, we return a byte array that represents a placeholder
                // Install-Package iTextSharp
                // Install-Package IronPdf (commercial)

                // Placeholder implementation - returns HTML bytes wrapped in basic PDF structure
                var pdfBytes = GeneratePlaceholderPdf(htmlContent);
                return pdfBytes;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(PdfHelper));
                log.Error("PDF generation failed", ex);
                throw;
            }
        }

        private static byte[] GeneratePlaceholderPdf(string htmlContent)
        {
            // Simplified PDF generation using HTML-to-text approach
            // In production, replace with proper PDF library
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    writer.WriteLine("%PDF-1.4");
                    writer.WriteLine("1 0 obj");
                    writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
                    writer.WriteLine("endobj");
                    writer.WriteLine("2 0 obj");
                    writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
                    writer.WriteLine("endobj");
                    writer.WriteLine("3 0 obj");
                    writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792]");
                    writer.WriteLine("   /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>");
                    writer.WriteLine("endobj");
                    writer.WriteLine("4 0 obj");
                    writer.WriteLine("<< /Length 44 >>");
                    writer.WriteLine("stream");
                    writer.WriteLine("BT /F1 12 Tf 100 700 Td (BusTravel - E-Ticket) Tj ET");
                    writer.WriteLine("endstream");
                    writer.WriteLine("endobj");
                    writer.WriteLine("5 0 obj");
                    writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
                    writer.WriteLine("endobj");
                    writer.WriteLine("xref");
                    writer.WriteLine("0 6");
                    writer.WriteLine("0000000000 65535 f ");
                    writer.WriteLine("0000000009 00000 n ");
                    writer.WriteLine("0000000058 00000 n ");
                    writer.WriteLine("0000000115 00000 n ");
                    writer.WriteLine("0000000266 00000 n ");
                    writer.WriteLine("0000000360 00000 n ");
                    writer.WriteLine("trailer << /Size 6 /Root 1 0 R >>");
                    writer.WriteLine("startxref");
                    writer.WriteLine("438");
                    writer.WriteLine("%%EOF");
                }
                return ms.ToArray();
            }
        }

        public static string SaveTicketPdf(int bookingId, string bookingNumber, byte[] pdfData)
        {
            try
            {
                var directory = HttpContext.Current.Server.MapPath("~/Content/tickets/");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fileName = $"Ticket_{bookingNumber}.pdf";
                var filePath = Path.Combine(directory, fileName);
                File.WriteAllBytes(filePath, pdfData);

                return $"~/Content/tickets/{fileName}";
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(PdfHelper));
                log.Error($"Failed to save PDF ticket for booking {bookingNumber}", ex);
                return null;
            }
        }
    }
}
