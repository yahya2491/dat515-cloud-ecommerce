using Ecommerce.Api.Models;
using Ecommerce.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CartService> _logger;

        public CartService(AppDbContext db, ILogger<CartService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _db.Carts.AsNoTracking().ToListAsync();
        }

        public async Task<Cart?> GetCartAsync(Guid cartId)
        {
            return await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<CartDto?> GetCartDtoAsync(Guid cartId)
        {
            var cart = await GetCartAsync(cartId);
            return cart == null ? null : MapToCartDto(cart);
        }

        public async Task<Cart> GetOrCreateUserCartAsync(Guid userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<CartDto> GetOrCreateUserCartDtoAsync(Guid userId)
        {
            var cart = await GetOrCreateUserCartAsync(userId);
            return MapToCartDto(cart);
        }

        public async Task<Cart> CreateCartAsync()
        {
            var cart = new Cart
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();

            return cart;
        }

        public async Task<CreateCartDto> CreateCartDtoAsync(Guid userId)
        {
            try
            {
                var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogWarning("CreateCartDtoAsync: User {UserId} not found", userId);
                    throw new InvalidOperationException("User not found");
                }

            var existingCart = await _db.Carts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (existingCart != null)
            {
                return new CreateCartDto
                {
                    Id = existingCart.Id,
                    UserId = existingCart.UserId,
                    CreatedAt = existingCart.CreatedAt,
                    UpdatedAt = existingCart.UpdatedAt,
                    Items = Array.Empty<object>(),
                    TotalPrice = 0
                };
            }

                var cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();

                return new CreateCartDto
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt,
                    Items = Array.Empty<object>(),
                    TotalPrice = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Cart?> AddItemAsync(Guid cartId, Guid productId, int quantity)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            Cart? updatedCart = null;

            await strategy.ExecuteAsync(async () =>
            {
                if (string.Equals(_db.Database.ProviderName, "Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase))
                {
                    var cart = await _db.Carts.FindAsync(cartId);
                    if (cart == null)
                        throw new InvalidOperationException("Cart not found");

                    var product = await _db.Products.FindAsync(productId);
                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    var existingItem = await _db.CartItems
                        .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        await _db.CartItems.AddAsync(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            UnitPrice = product.Price,
                            Quantity = quantity
                        });
                    }

                    cart.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();

                    updatedCart = await _db.Carts
                        .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(c => c.Id == cartId);
                }
                else
                {
                    await using var tx = await _db.Database.BeginTransactionAsync();

                    var cart = await _db.Carts.FindAsync(cartId);
                    if (cart == null)
                        throw new InvalidOperationException("Cart not found");

                    var product = await _db.Products.FindAsync(productId);
                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    var existingItem = await _db.CartItems
                        .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        await _db.CartItems.AddAsync(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            UnitPrice = product.Price,
                            Quantity = quantity
                        });
                    }

                    cart.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();

                    updatedCart = await _db.Carts
                        .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(c => c.Id == cartId);

                    await tx.CommitAsync();
                }
            });

            return updatedCart;
        }

        public async Task<CartDto?> AddItemDtoAsync(Guid cartId, Guid productId, int quantity)
        {
            var cart = await AddItemAsync(cartId, productId, quantity);
            return cart == null ? null : MapToCartDto(cart);
        }

        public async Task<Cart?> AddItemForUserAsync(Guid userId, Guid productId, int quantity)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            Cart? updatedCart = null;

            await strategy.ExecuteAsync(async () =>
            {
                if (string.Equals(_db.Database.ProviderName, "Microsoft.EntityFrameworkCore.InMemory", StringComparison.OrdinalIgnoreCase))
                {
                    var user = await _db.Users.FindAsync(userId);
                    if (user == null)
                        throw new InvalidOperationException("User not found");

                    var product = await _db.Products.FindAsync(productId);
                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                        await _db.Carts.AddAsync(cart);
                        await _db.SaveChangesAsync();
                    }

                    var existingItem = await _db.CartItems
                        .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        await _db.CartItems.AddAsync(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = productId,
                            ProductName = product.Name,
                            UnitPrice = product.Price,
                            Quantity = quantity
                        });
                    }

                    cart.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();

                    updatedCart = await _db.Carts
                        .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == cart.Id);
                }
                else
                {
                    await using var tx = await _db.Database.BeginTransactionAsync();

                    var user = await _db.Users.FindAsync(userId);
                    if (user == null)
                        throw new InvalidOperationException("User not found");

                    var product = await _db.Products.FindAsync(productId);
                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
                    if (cart == null)
                    {
                        cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
                        await _db.Carts.AddAsync(cart);
                        await _db.SaveChangesAsync();
                    }

                    var existingItem = await _db.CartItems
                        .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        await _db.CartItems.AddAsync(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = productId,
                            ProductName = product.Name,
                            UnitPrice = product.Price,
                            Quantity = quantity
                        });
                    }

                    cart.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();

                    updatedCart = await _db.Carts
                        .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == cart.Id);

                    await tx.CommitAsync();
                }
            });

            return updatedCart;
        }

        public async Task<CartDto?> AddItemForUserDtoAsync(Guid userId, Guid productId, int quantity)
        {
            var cart = await AddItemForUserAsync(userId, productId, quantity);
            return cart == null ? null : MapToCartDto(cart);
        }

        public async Task<bool> RemoveItemAsync(Guid cartId, Guid productId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return false;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                return false;

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return false;

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignCartToUserAsync(Guid cartId, Guid userId)
        {
            var cart = await _db.Carts.FindAsync(cartId);
            if (cart == null)
                return false;

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return false;

            cart.UserId = userId;
            cart.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return true;
        }

        private static CartDto MapToCartDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    CartId = i.CartId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Total = i.Total,
                    Product = new ProductDto
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Price = i.Product.Price,
                        Category = i.Product.Category
                    }
                }),
                TotalPrice = cart.TotalPrice
            };
        }
    }
}