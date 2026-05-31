using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Tests.TestHelpers
{
    public static class InMemoryDbFactory
    {
        public static AppDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(options);
            return context;
        }
    }
}
