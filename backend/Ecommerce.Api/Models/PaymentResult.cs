using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Api.Models;

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? PaymentId { get; set; }
    }