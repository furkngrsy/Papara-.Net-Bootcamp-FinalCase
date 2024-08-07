using Papara_Final_Project.DTOs;
using System;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<bool> ProcessPayment(PaymentDTO paymentDto)
        {
            if (string.IsNullOrWhiteSpace(paymentDto.Name) || paymentDto.CardNumber.Length != 16 || !IsValidExpiryDate(paymentDto.ExpiryDate) || paymentDto.CVV.Length != 3)
            {
                return false;
            }

            // Burada gerçek ödeme işlemi gerçekleştirilmelidir.
            return await Task.FromResult(true);
        }

        private bool IsValidExpiryDate(string expiryDate)
        {
            if (!DateTime.TryParseExact(expiryDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return false;
            }

            return date > DateTime.Now;
        }
    }
}
