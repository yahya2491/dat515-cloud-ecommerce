using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Ecommerce.Api.Services;


namespace Ecommerce.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly IUserService _userService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(AppDbContext db, IUserService userService, ILogger<PaymentService> logger)
        {
            _db = db;
            _userService = userService;
            _logger = logger;
        }
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "User with ID " + request.UserId + " not found."
                    };
                }
                var options = new PaymentIntentCreateOptions
                {
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Description = request.Description,
                    ReceiptEmail = request.ReceiptEmail,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);


                return new PaymentResult
                {
                    Success = true,
                    PaymentId = paymentIntent.Id,
                    Message = "Customer with a ID of " + request.UserId + " was charged successfully."
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error while processing payment for user {UserId}", request.UserId);
                return new PaymentResult
                {
                    Success = false,
                    Message = $"Stripe error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing payment for user {UserId}", request.UserId);
                return new PaymentResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}