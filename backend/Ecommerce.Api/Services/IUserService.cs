using System.Collections.Generic;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string username, string nickname, string password);
        Task<User?> LoginAsync(string username, string password);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
    }
}
