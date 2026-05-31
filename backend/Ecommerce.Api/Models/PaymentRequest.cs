using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
//zz
namespace Ecommerce.Api.Models;
public class PaymentRequest
{
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Amount must be positive.")]
    public long Amount { get; set; }
    [Required]
    public string Currency { get; set; } = "usd";
    [Required]
    public string Description { get; set; } = "Test Payment";
    [Required]
    public Guid UserId { get; set; }
    [Required]
    [EmailAddress]
    public string? ReceiptEmail { get; set; }
}