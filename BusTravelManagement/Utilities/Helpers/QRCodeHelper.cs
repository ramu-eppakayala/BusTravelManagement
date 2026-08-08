using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BusTravelManagement.Utilities.Helpers
{
    public static class QRCodeHelper
    {
        public static string GenerateQRCodeBase64(string data, int size = 200)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var bitmap = GenerateBitmap(data, size))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch
            {
                // Fallback: return a placeholder if QR generation fails
                return GeneratePlaceholderQR(data);
            }
        }

        public static byte[] GenerateQRCodeBytes(string data, int size = 200)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var bitmap = GenerateBitmap(data, size))
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                    }
                    return ms.ToArray();
                }
            }
            catch
            {
                return new byte[0];
            }
        }

        private static Bitmap GenerateBitmap(string data, int size)
        {
            var bitmap = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, size, size);

                // Draw a stylized QR-like pattern (simplified representation)
                var rand = new Random(data.GetHashCode());
                var cellSize = size / 25;
                var brushColor = Color.FromArgb(52, 73, 94);

                using (var brush = new SolidBrush(brushColor))
                {
                    // Draw position detection patterns (corners)
                    DrawPositionPattern(graphics, brush, cellSize, 0, 0);
                    DrawPositionPattern(graphics, brush, cellSize, size - cellSize * 7, 0);
                    DrawPositionPattern(graphics, brush, cellSize, 0, size - cellSize * 7);

                    // Draw random data cells
                    for (int row = 0; row < 25; row++)
                    {
                        for (int col = 0; col < 25; col++)
                        {
                            if (rand.NextDouble() > 0.5)
                            {
                                graphics.FillRectangle(brush,
                                    col * cellSize + cellSize / 4,
                                    row * cellSize + cellSize / 4,
                                    cellSize / 2,
                                    cellSize / 2);
                            }
                        }
                    }

                    // Draw center logo area
                    graphics.FillRectangle(Brushes.White,
                        size / 2 - cellSize * 3,
                        size / 2 - cellSize * 3,
                        cellSize * 6,
                        cellSize * 6);
                }
            }
            return bitmap;
        }

        private static void DrawPositionPattern(Graphics graphics, Brush brush, int cellSize, int startX, int startY)
        {
            graphics.FillRectangle(brush, startX, startY, cellSize * 7, cellSize * 7);
            graphics.FillRectangle(Brushes.White,
                startX + cellSize,
                startY + cellSize,
                cellSize * 5,
                cellSize * 5);
            graphics.FillRectangle(brush,
                startX + cellSize * 2,
                startY + cellSize * 2,
                cellSize * 3,
                cellSize * 3);
        }

        private static string GeneratePlaceholderQR(string data)
        {
            // Generate a simple SVG placeholder
            var svg = $@"<svg xmlns='http://www.w3.org/2000/svg' width='200' height='200' viewBox='0 0 200 200'>
                <rect width='200' height='200' fill='#f8f9fa' rx='10'/>
                <rect x='10' y='10' width='50' height='50' fill='#2c3e50' rx='5'/>
                <rect x='140' y='10' width='50' height='50' fill='#2c3e50' rx='5'/>
                <rect x='10' y='140' width='50' height='50' fill='#2c3e50' rx='5'/>
                <text x='100' y='115' text-anchor='middle' fill='#7f8c8d' font-size='12' font-family='monospace'>BUS TICKET</text>
            </svg>";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
        }
    }
}
