using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> UpdateUserAsync(Guid id, string newName, string newRole);
        Task<bool> DeleteUserAsync(Guid id);
        Task<object> GetStatsAsync();
        void TrackProductView(Guid productId);
    }
}
