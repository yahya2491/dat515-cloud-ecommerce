using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext db, ILogger<ProductService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _db.Products.AsNoTracking().ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _db.Products.FindAsync(id);

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product.Category))
                {
                    product.Category = CategoryPredictor.Predict(product.Name, product.Description ?? string.Empty);
                }

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Created product {ProductId}", product.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {Name}", product?.Name);
                throw;
            }
        }

        public async Task<Product?> UpdateAsync(Guid id, Product updated)
        {
            if (id != updated.Id)
                return null;

            var existing = await _db.Products.FindAsync(id);
            if (existing == null) 
                return null;

            bool nameChanged = existing.Name != updated.Name;
            bool priceChanged = existing.Price != updated.Price;

            existing.Name = updated.Name;
            existing.Price = updated.Price;
            existing.Stock = updated.Stock;
            existing.Description = updated.Description;
            var updatedCategory = updated.Category;
            if (string.IsNullOrWhiteSpace(updatedCategory))
            {
                updatedCategory = CategoryPredictor.Predict(existing.Name, existing.Description ?? string.Empty);
            }
            existing.Category = updatedCategory;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving product changes for {ProductId}", id);
                throw;
            }

            if (nameChanged || priceChanged)
            {
                var affectedItems = await _db.CartItems
                    .Where(ci => ci.ProductId == existing.Id)
                    .ToListAsync();

                foreach (var item in affectedItems)
                {
                    if (nameChanged)
                        item.ProductName = existing.Name;

                    if (priceChanged)
                        item.UnitPrice = existing.Price;
                }

                if (affectedItems.Count > 0)
                {
                    try
                    {
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing cart items after product update {ProductId}", id);
                        throw;
                    }
                }
            }

            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing == null) return false;

            _db.Products.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
