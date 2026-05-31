namespace Ecommerce.Api.Models.Dtos
{
    public record ProductCreateDto(string Name, decimal Price, int Stock, string ImageUrl, string Category);
    public record ProductUpdateDto(int Id, string Name, decimal Price, int Stock, string ImageUrl, string Category);
}
