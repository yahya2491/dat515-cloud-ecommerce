using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Api.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } = null!;

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;
    }
}
