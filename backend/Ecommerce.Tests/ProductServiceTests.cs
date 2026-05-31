using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce.Tests
{
    public class SimpleProductService : IProductService
    {
        private readonly List<Product> _store = new();

        public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult<IEnumerable<Product>>(_store);

        public Task<Product?> GetByIdAsync(Guid id) => Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = product.Id == Guid.Empty ? Guid.NewGuid() : product.Id;
            _store.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product?> UpdateAsync(Guid id, Product updated)
        {
            var ex = _store.FirstOrDefault(p => p.Id == id);
            if (ex == null) return Task.FromResult<Product?>(null);
            if (id != updated.Id) return Task.FromResult<Product?>(null);
            ex.Name = updated.Name;
            ex.Price = updated.Price;
            ex.Stock = updated.Stock;
            ex.Description = updated.Description;
            return Task.FromResult<Product?>(ex);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var ex = _store.FirstOrDefault(p => p.Id == id);
            if (ex == null) return Task.FromResult(false);
            _store.Remove(ex);
            return Task.FromResult(true);
        }
    }

    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateGetUpdateDeleteFlow_Works_OnSimpleFake()
        {
            var svc = new SimpleProductService();

            var p = new Product { Name = "Milk", Price = 3.5m, Stock = 10 };
            var created = await svc.CreateAsync(p);
            Assert.NotEqual(Guid.Empty, created.Id);

            var found = await svc.GetByIdAsync(created.Id);
            Assert.NotNull(found);

            var updated = new Product { Id = created.Id, Name = "Milk 2", Price = 4m, Stock = 9 };
            var res = await svc.UpdateAsync(created.Id, updated);
            Assert.NotNull(res);
            Assert.Equal("Milk 2", res!.Name);

            var deleted = await svc.DeleteAsync(created.Id);
            Assert.True(deleted);

            var after = (await svc.GetAllAsync()).ToList();
            Assert.Empty(after);
        }

        [Fact]
        public async Task CreateAsync_PredictsCategory_WhenMissing()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"products-{Guid.NewGuid()}")
                .Options;

            using var db = new AppDbContext(options);
            var logger = NullLogger<ProductService>.Instance;
            var svc = new ProductService(db, logger);

            var created = await svc.CreateAsync(new Product
            {
                Name = "Lemon",
                Price = 0.99m,
                Description = "Fresh citrus fruit.",
                Stock = 5
            });

            Assert.Equal("Produce", created.Category);

            var stored = await db.Products.FindAsync(created.Id);
            Assert.NotNull(stored);
            Assert.Equal("Produce", stored!.Category);
        }

        [Fact]
        public async Task CreateAsync_HonorsExplicitCategory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase($"products-explicit-{Guid.NewGuid()}")
                .Options;

            using var db = new AppDbContext(options);
            var logger = NullLogger<ProductService>.Instance;
            var svc = new ProductService(db, logger);

            var created = await svc.CreateAsync(new Product
            {
                Name = "Banana Smoothie",
                Price = 4.25m,
                Description = "Creamy smoothie with fresh bananas.",
                Stock = 12,
                Category = "General"
            });

            Assert.Equal("General", created.Category);

            var stored = await db.Products.FindAsync(created.Id);
            Assert.NotNull(stored);
            Assert.Equal("General", stored!.Category);
        }
    }
}
