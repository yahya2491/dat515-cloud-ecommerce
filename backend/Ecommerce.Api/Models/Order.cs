using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
