using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ecommerce.Api.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<CartItem> Items { get; set; } = new();

        public decimal TotalPrice =>
            Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
