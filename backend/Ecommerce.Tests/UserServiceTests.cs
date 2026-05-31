using Ecommerce.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce.Tests
{
    public class SimpleUserService : Ecommerce.Api.Services.IUserService
    {
        private readonly List<User> _users = new();

        public Task<User?> RegisterAsync(string username, string nickname, string password)
        {
            if (_users.Any(u => u.Username == username)) return Task.FromResult<User?>(null);
            var user = new User { Id = Guid.NewGuid(), Username = username, Nickname = nickname, PasswordHash = password, Role = username.ToLower() == "admin" ? "Admin" : "User" };
            _users.Add(user);
            return Task.FromResult<User?>(user);
        }

        public Task<User?> LoginAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<User>>(_users.ToList());
        }

        public Task<User?> GetByIdAsync(Guid id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }

    public class UserServiceTests
    {
        [Fact]
        public async Task Register_Login_GetById_Works_OnSimpleFake()
        {
            var svc = new SimpleUserService();

            var u = await svc.RegisterAsync("alice", "Alice", "pw");
            Assert.NotNull(u);
            Assert.Equal("alice", u!.Username);

            var logged = await svc.LoginAsync("alice", "pw");
            Assert.NotNull(logged);

            var found = await svc.GetByIdAsync(u.Id);
            Assert.NotNull(found);
            Assert.Equal(u.Id, found!.Id);
        }

        [Fact]
        public async Task Register_ReturnsNull_ForDuplicate()
        {
            var svc = new SimpleUserService();
            var first = await svc.RegisterAsync("dup", "D", "pw");
            Assert.NotNull(first);

            var second = await svc.RegisterAsync("dup", "D2", "pw2");
            Assert.Null(second);
        }
    }
}
