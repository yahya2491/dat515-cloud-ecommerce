using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace Ecommerce.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AdminService> _logger;

        private static readonly Counter ProductViewCounter =
            Metrics.CreateCounter("product_views_total", "Total number of product views", new[] { "product_id" });

        public AdminService(AppDbContext db, ILogger<AdminService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _db.Users.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by id {UserId}", id);
                throw;
            }
        }

        public async Task<User?> UpdateUserAsync(Guid id, string newName, string newRole)
        {
            try
            {
                var user = await _db.Users.FindAsync(id);
                if (user == null) return null;

                user.Username = newName;
                user.Role = newRole;

                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _db.Users.FindAsync(id);
                if (user == null) return false;

                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                throw;
            }
        }

        public async Task<object> GetStatsAsync()
        {
            try
            {
                var totalUsers = await _db.Users.CountAsync();
                var totalProducts = await _db.Products.CountAsync();

                return new
                {
                    totalUsers,
                    totalProducts,
                    totalProductViews = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stats");
                throw;
            }
        }

        public void TrackProductView(Guid productId)
        {
            ProductViewCounter.WithLabels(productId.ToString()).Inc();
        }
    }
}
