using Ecommerce.Api.Services;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    }
}
