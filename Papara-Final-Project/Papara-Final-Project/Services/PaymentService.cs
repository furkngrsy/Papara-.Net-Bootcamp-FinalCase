using FluentValidation;
using Papara_Final_Project.DTOs;


namespace Papara_Final_Project.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly IValidator<PaymentDTO> _paymentValidator;

        public PaymentService(IValidator<PaymentDTO> paymentValidator)
        {
            _paymentValidator = paymentValidator;
        }

        public async Task<bool> ProcessPayment(PaymentDTO paymentDto)
        {
            var validationResult = await _paymentValidator.ValidateAsync(paymentDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors); 
            }

            return await Task.FromResult(true);
        }
        
    }
}
