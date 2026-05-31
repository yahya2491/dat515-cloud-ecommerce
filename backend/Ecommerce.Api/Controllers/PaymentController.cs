using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Ecommerce.Api.Attributes;
using Ecommerce.Api.Models;
using Ecommerce.Api.Services;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("checkout/{userId}")]
        public async Task<ActionResult> Checkout([FromBody] PaymentRequest request, Guid userId)
        {
            _logger.LogInformation("Checkout called for user {UserId} amount={Amount}", userId, request?.Amount);
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Checkout called with null request for user {UserId}", userId);
                    return BadRequest("Request body is required");
                }

                request.UserId = userId;
                var result = await _paymentService.ProcessPaymentAsync(request);
                if (!result.Success)
                {
                    _logger.LogWarning("Payment failed for user {UserId}: {Message}", userId, result.Message);
                    return BadRequest(result.Message);
                }

                _logger.LogInformation("Payment succeeded for user {UserId}, paymentId={PaymentId}", userId, result.PaymentId);
                return Ok(new { result.PaymentId, result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during checkout for user {UserId}", userId);
                return StatusCode(500, "Error processing payment");
            }
        }
    }
}
