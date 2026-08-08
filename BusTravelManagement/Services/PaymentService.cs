using System;
using System.Linq;
using BusTravelManagement.Models.Entities;
using BusTravelManagement.Models.ViewModels;
using BusTravelManagement.Repositories.Interfaces;
using BusTravelManagement.Services.Interfaces;

namespace BusTravelManagement.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PaymentResultViewModel ProcessPayment(PaymentRequestViewModel model)
        {
            var result = new PaymentResultViewModel
            {
                Success = false,
                BookingNumber = model.BookingNumber
            };

            try
            {
                var booking = _unitOfWork.Bookings.GetByBookingNumber(model.BookingNumber);
                if (booking == null)
                {
                    result.Message = "Booking not found.";
                    return result;
                }

                // Mock payment gateway - always succeeds
                if (model.PaymentGateway == "Mock")
                {
                    var paymentReference = GeneratePaymentReference();
                    var gatewayTransactionId = $"MOCK_{DateTime.UtcNow:yyyyMMddHHmmss}_{new Random().Next(100000, 999999)}";

                    var payment = new Payment
                    {
                        BookingId = booking.Id,
                        PaymentReference = paymentReference,
                        PaymentMethod = model.PaymentMethod,
                        PaymentGateway = model.PaymentGateway,
                        GatewayTransactionId = gatewayTransactionId,
                        Amount = model.Amount > 0 ? model.Amount : booking.NetAmount,
                        Currency = "INR",
                        PaymentStatus = "Success",
                        PaidAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };

                    _unitOfWork.Payments.Add(payment);
                    _unitOfWork.SaveChanges();

                    result.Success = true;
                    result.Message = "Payment processed successfully.";
                    result.PaymentReference = paymentReference;
                    result.GatewayTransactionId = gatewayTransactionId;
                    result.Amount = payment.Amount;
                    result.PaymentStatus = "Success";
                    result.BookingNumber = model.BookingNumber;
                }
                else
                {
                    // For non-mock gateways, simulate processing
                    var paymentReference = GeneratePaymentReference();
                    var gatewayTxId = $"{model.PaymentGateway}_{DateTime.UtcNow:yyyyMMddHHmmss}_{new Random().Next(100000, 999999)}";

                    var payment = new Payment
                    {
                        BookingId = booking.Id,
                        PaymentReference = paymentReference,
                        PaymentMethod = model.PaymentMethod,
                        PaymentGateway = model.PaymentGateway,
                        GatewayTransactionId = gatewayTxId,
                        Amount = model.Amount > 0 ? model.Amount : booking.NetAmount,
                        Currency = "INR",
                        PaymentStatus = "Success",
                        PaidAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };

                    _unitOfWork.Payments.Add(payment);
                    _unitOfWork.SaveChanges();

                    result.Success = true;
                    result.Message = "Payment processed successfully.";
                    result.PaymentReference = paymentReference;
                    result.GatewayTransactionId = gatewayTxId;
                    result.Amount = payment.Amount;
                    result.PaymentStatus = "Success";
                    result.BookingNumber = model.BookingNumber;
                }
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(PaymentService));
                log.Error("ProcessPayment error", ex);
                result.Message = "An error occurred while processing payment.";
            }

            return result;
        }

        public PaymentStatusViewModel CheckPaymentStatus(string paymentReference)
        {
            try
            {
                var payment = _unitOfWork.Payments.GetByPaymentReference(paymentReference);
                if (payment == null) return null;

                return new PaymentStatusViewModel
                {
                    PaymentReference = payment.PaymentReference,
                    PaymentStatus = payment.PaymentStatus,
                    Amount = payment.Amount,
                    PaidAt = payment.PaidAt,
                    FailureReason = payment.FailureReason
                };
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(PaymentService));
                log.Error("CheckPaymentStatus error", ex);
                return null;
            }
        }

        public RefundResultViewModel ProcessRefund(string bookingNumber, string reason)
        {
            var result = new RefundResultViewModel { Success = false };

            try
            {
                var booking = _unitOfWork.Bookings.GetByBookingNumber(bookingNumber);
                if (booking == null)
                {
                    result.Message = "Booking not found.";
                    return result;
                }

                var payment = _unitOfWork.Payments.GetBookingPayments(booking.Id)
                    .FirstOrDefault(p => p.PaymentStatus == "Success");

                if (payment == null)
                {
                    result.Message = "No successful payment found for this booking.";
                    return result;
                }

                if (payment.PaymentStatus == "Refunded")
                {
                    result.Message = "Payment has already been refunded.";
                    return result;
                }

                // Determine refund type
                var refundType = booking.RefundAmount >= booking.NetAmount ? "Full" : "Partial";

                var refundReference = $"REF{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

                var refund = new Refund
                {
                    PaymentId = payment.Id,
                    BookingId = booking.Id,
                    RefundReference = refundReference,
                    Amount = booking.RefundAmount,
                    RefundType = refundType,
                    Reason = reason,
                    ProcessedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Query<Refund>().Add(refund);

                // Update payment status
                payment.PaymentStatus = "Refunded";
                payment.RefundedAt = DateTime.UtcNow;

                // Update booking status
                booking.BookingStatus = "Refunded";
                booking.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SaveChanges();

                result.Success = true;
                result.Message = "Refund processed successfully.";
                result.RefundReference = refundReference;
                result.RefundAmount = booking.RefundAmount;
                result.RefundType = refundType;
            }
            catch (Exception ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(PaymentService));
                log.Error("ProcessRefund error", ex);
                result.Message = "An error occurred while processing refund.";
            }

            return result;
        }

        private string GeneratePaymentReference()
        {
            var prefix = "PAY";
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(10000, 99999).ToString();
            return $"{prefix}{timestamp}{random}";
        }
    }
}
