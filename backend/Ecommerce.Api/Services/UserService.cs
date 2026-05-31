using System.Collections.Generic;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Ecommerce.Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<User?> RegisterAsync(string username, string nickname, string password)
        {
            try
            {
                if (await _db.Users.AnyAsync(u => u.Username == username))
                {
                    _logger.LogWarning("Register attempted for existing username {Username}", username);
                    return null;
                }

                var user = new User();
                user.Username = username;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                if (username.ToLower() == "admin")
                    user.Role = "Admin";
                else
                    user.Role = "User";

                user.Nickname = nickname;

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Registered new user {Username} id={UserId}", username, user.Id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", username);
                throw;
            }
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    _logger.LogWarning("Login failed - user not found {Username}", username);
                    return null;
                }

                bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (!valid)
                    _logger.LogWarning("Login failed - invalid password for {Username}", username);

                return valid ? user : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _db.Users.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id {UserId}", id);
                throw;
            }
        }
    }
}
