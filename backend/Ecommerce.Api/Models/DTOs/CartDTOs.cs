namespace Ecommerce.Api.Models.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
    }

    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public ProductDto Product { get; set; } = null!;
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Category { get; set; }
    }

    public class AddItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class CreateCartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public object[] Items { get; set; } = Array.Empty<object>();
        public decimal TotalPrice { get; set; } = 0;
    }
}