using Ecommerce.Api.Models;

namespace Ecommerce.Tests
{
	public class MiniCartService
	{
		private readonly Dictionary<Guid, Cart> _carts = new();

		public Task<Cart> CreateCartAsync()
		{
			var c = new Cart { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
			_carts[c.Id] = c;
			return Task.FromResult(c);
		}

		public Task<Cart> GetOrCreateUserCartAsync(Guid userId)
		{
			var c = _carts.Values.FirstOrDefault(x => x.UserId == userId);
			if (c == null)
			{
				c = new Cart { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
				_carts[c.Id] = c;
			}
			return Task.FromResult(c);
		}

		public Task<Cart?> GetCartAsync(Guid id)
		{
			_carts.TryGetValue(id, out var c);
			return Task.FromResult(c);
		}

		public Task<bool> AssignCartToUserAsync(Guid cartId, Guid userId)
		{
			if (!_carts.TryGetValue(cartId, out var c)) return Task.FromResult(false);
			c.UserId = userId;
			c.UpdatedAt = DateTime.UtcNow;
			return Task.FromResult(true);
		}

		public Task<bool> ClearCartAsync(Guid cartId)
		{
			if (!_carts.TryGetValue(cartId, out var c)) return Task.FromResult(false);
			c.Items.Clear();
			c.UpdatedAt = DateTime.UtcNow;
			return Task.FromResult(true);
		}
	}

	public class CartServiceTests
	{
		[Fact]
		public async Task CreateCart_CreatesNewCart()
		{
			var svc = new MiniCartService();
			var c = await svc.CreateCartAsync();
			Assert.NotEqual(Guid.Empty, c.Id);
		}

		[Fact]
		public async Task GetOrCreateUserCart_CreatesAndReturnsSame()
		{
			var svc = new MiniCartService();
			var userId = Guid.NewGuid();
			var first = await svc.GetOrCreateUserCartAsync(userId);
			var second = await svc.GetOrCreateUserCartAsync(userId);
			Assert.Equal(first.Id, second.Id);
			Assert.Equal(userId, second.UserId);
		}

		[Fact]
		public async Task AssignCartToUser_UpdatesUserId()
		{
			var svc = new MiniCartService();
			var cart = await svc.CreateCartAsync();
			var user = Guid.NewGuid();
			var ok = await svc.AssignCartToUserAsync(cart.Id, user);
			Assert.True(ok);
			var refreshed = await svc.GetCartAsync(cart.Id);
			Assert.Equal(user, refreshed!.UserId);
		}

		[Fact]
		public async Task ClearCart_RemovesItems()
		{
			var svc = new MiniCartService();
			var cart = await svc.CreateCartAsync();
			cart.Items.Add(new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, ProductId = Guid.NewGuid(), ProductName = "P", UnitPrice = 1, Quantity = 1 });
			var ok = await svc.ClearCartAsync(cart.Id);
			Assert.True(ok);
			var refreshed = await svc.GetCartAsync(cart.Id);
			Assert.Empty(refreshed!.Items);
		}
	}
}
