using Papara_Final_Project.DTOs;

namespace Papara_Final_Project.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPayment(PaymentDTO paymentDto);
    }
}
