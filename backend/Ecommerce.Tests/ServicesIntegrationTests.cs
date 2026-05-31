using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Ecommerce.Tests.TestHelpers;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;

namespace Ecommerce.Tests
{
    public class ServicesIntegrationTests
    {
        [Fact]
        public async Task UserService_CRUD_Works()
        {
            var db = InMemoryDbFactory.Create("UserService_CRUD");
            var svc = new UserService(db, NullLogger<UserService>.Instance);

            var u = await svc.RegisterAsync("u1", "User1", "pw");
            Assert.NotNull(u);

            var logged = await svc.LoginAsync("u1", "pw");
            Assert.NotNull(logged);

            var fetched = await svc.GetByIdAsync(u!.Id);
            Assert.NotNull(fetched);
            Assert.Equal(u.Id, fetched!.Id);
        }

        [Fact]
        public async Task ProductService_CRUD_Works()
        {
            var db = InMemoryDbFactory.Create("ProductService_CRUD");
            var svc = new ProductService(db, NullLogger<ProductService>.Instance);

            var p = new Product { Name = "Bread", Price = 2m, Stock = 5 };
            var created = await svc.CreateAsync(p);
            Assert.NotEqual(Guid.Empty, created.Id);

            var found = await svc.GetByIdAsync(created.Id);
            Assert.NotNull(found);

            var updated = new Product { Id = created.Id, Name = "Bread 2", Price = 2.5m, Stock = 4 };
            var res = await svc.UpdateAsync(created.Id, updated);
            Assert.NotNull(res);
            Assert.Equal("Bread 2", res!.Name);

            var del = await svc.DeleteAsync(created.Id);
            Assert.True(del);
        }

        [Fact]
        public async Task CartService_Workflow_Works()
        {
            var db = InMemoryDbFactory.Create("CartService_Workflow");
            var user = new User { Id = Guid.NewGuid(), Username = "testuser", PasswordHash = "pw" };
            db.Users.Add(user);

            var product = new Product { Id = Guid.NewGuid(), Name = "Soda", Price = 1m };
            db.Products.Add(product);

            await db.SaveChangesAsync();

            var svc = new CartService(db, NullLogger<CartService>.Instance);

            // create cart for user
            var cartDto = await svc.CreateCartDtoAsync(user.Id);
            Assert.Equal(user.Id, cartDto.UserId);

            // add item for user
            var added = await svc.AddItemForUserDtoAsync(user.Id, product.Id, 2);
            Assert.NotNull(added);
            Assert.Equal(1, added!.Items.Count());
            Assert.Equal(product.Id, added.Items.First().ProductId);

            // get or create user cart
            var cart = await svc.GetOrCreateUserCartAsync(user.Id);
            Assert.Equal(user.Id, cart.UserId);

            // remove item
            var okRemove = await svc.RemoveItemAsync(cart.Id, product.Id);
            Assert.True(okRemove);

            // clear cart
            var okClear = await svc.ClearCartAsync(cart.Id);
            Assert.True(okClear);
        }

        [Fact]
        public async Task AdminService_StatsAndCrud_Works()
        {
            var db = InMemoryDbFactory.Create("AdminService_Stats");
            var admin = new AdminService(db, NullLogger<AdminService>.Instance);

            // seed a user and product
            var u = new User { Id = Guid.NewGuid(), Username = "a", PasswordHash = "pw" };
            db.Users.Add(u);
            var p = new Product { Id = Guid.NewGuid(), Name = "Prod", Price = 1m };
            db.Products.Add(p);
            await db.SaveChangesAsync();

            var users = await admin.GetAllUsersAsync();
            Assert.NotEmpty(users);

            var got = await admin.GetUserByIdAsync(u.Id);
            Assert.NotNull(got);

            var stats = await admin.GetStatsAsync();
            Assert.NotNull(stats);

            var deleted = await admin.DeleteUserAsync(u.Id);
            Assert.True(deleted);
        }
    }
}
