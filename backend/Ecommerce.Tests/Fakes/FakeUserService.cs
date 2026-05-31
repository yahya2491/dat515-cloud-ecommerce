using System;
using System.Collections.Generic;
using Ecommerce.Api.Models;
using Ecommerce.Api.Services;

namespace Ecommerce.Tests.Fakes
{
    public class FakeUserService : IUserService
    {
        private readonly User? _user;
        private readonly bool _throwOnGet;

        public FakeUserService(User? user = null, bool throwOnGet = false)
        {
            _user = user;
            _throwOnGet = throwOnGet;
        }

        public Task<User?> RegisterAsync(string username, string nickname, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User?> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            if (_throwOnGet) throw new Exception("boom");

            IEnumerable<User> users = _user != null
                ? new[] { _user }
                : Array.Empty<User>();

            return Task.FromResult(users);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            if (_throwOnGet) throw new Exception("boom");
            return Task.FromResult(_user);
        }
    }
}
