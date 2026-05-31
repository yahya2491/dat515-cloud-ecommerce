using Ecommerce.Api.Models;
using Ecommerce.Api.Models.DTOs;

namespace Ecommerce.Api.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllAsync();
        Task<Cart?> GetCartAsync(Guid cartId);
        Task<CartDto?> GetCartDtoAsync(Guid cartId);
        Task<Cart> GetOrCreateUserCartAsync(Guid userId);
        Task<CartDto> GetOrCreateUserCartDtoAsync(Guid userId);
        Task<Cart> CreateCartAsync();
        Task<CreateCartDto> CreateCartDtoAsync(Guid userId);
        Task<Cart?> AddItemAsync(Guid cartId, Guid productId, int quantity);
        Task<CartDto?> AddItemDtoAsync(Guid cartId, Guid productId, int quantity);
        Task<Cart?> AddItemForUserAsync(Guid userId, Guid productId, int quantity);
        Task<CartDto?> AddItemForUserDtoAsync(Guid userId, Guid productId, int quantity);
        Task<bool> RemoveItemAsync(Guid cartId, Guid productId);
        Task<bool> ClearCartAsync(Guid cartId);
        Task<bool> AssignCartToUserAsync(Guid cartId, Guid userId);
    }
}