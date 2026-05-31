using Microsoft.AspNetCore.Mvc;
using Ecommerce.Api.Models;
using Ecommerce.Api.Models.DTOs;
using Ecommerce.Api.Services;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetAll()
        {
            _logger.LogInformation("GetAll carts called");
            try
            {
                var items = await _cartService.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all carts");
                return StatusCode(500, "Error retrieving carts");
            }
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<CartDto>> GetCart(Guid cartId)
        {
            _logger.LogInformation("GetCart {CartId} called", cartId);
            try
            {
                var cartDto = await _cartService.GetCartDtoAsync(cartId);

                if (cartDto == null)
                {
                    _logger.LogWarning("Cart {CartId} not found", cartId);
                    return NotFound("Cart not found");
                }

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart {CartId}", cartId);
                return StatusCode(500, "Error retrieving cart");
            }
        }

        [HttpPost("user/{userId}/add")]
        public async Task<ActionResult<CartDto>> AddItemForUser(Guid userId, [FromBody] AddItemDto dto)
        {
            _logger.LogInformation("AddItemForUser called for User {UserId} Product {ProductId} Quantity {Quantity}", userId, dto.ProductId, dto.Quantity);
            try
            {
                var cartDto = await _cartService.AddItemForUserDtoAsync(userId, dto.ProductId, dto.Quantity);

                if (cartDto == null)
                {
                    _logger.LogWarning("AddItemForUser failed for User {UserId}", userId);
                    return BadRequest("Could not add item to cart");
                }

                return Ok(cartDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("User not found"))
            {
                _logger.LogWarning(ex, "User {UserId} not found when adding item", userId);
                return NotFound("User not found");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Product not found"))
            {
                _logger.LogWarning(ex, "Product {ProductId} not found when adding to user {UserId}", dto.ProductId, userId);
                return NotFound("Product not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item for user {UserId}", userId);
                return StatusCode(500, $"Error adding item: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartDto>> GetOrCreateUserCart(Guid userId)
        {
            _logger.LogInformation("GetOrCreateUserCart called for {UserId}", userId);
            try
            {
                var cartDto = await _cartService.GetOrCreateUserCartDtoAsync(userId);
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating cart for user {UserId}", userId);
                return StatusCode(500, $"Error getting user cart: {ex.Message}");
            }
        }

        [Obsolete("Use POST /api/Cart/create/{userId} instead")]
        [HttpPost("create")]
        public ActionResult CreateCartDeprecated()
        {
            return BadRequest("Guest carts are not supported. Use POST /api/Cart/create/{userId}.");
        }

        [HttpPost("create/{userId}")]
        public async Task<ActionResult<CreateCartDto>> CreateCartForUser(Guid userId)
        {
            _logger.LogInformation("CreateCartForUser called for {UserId}", userId);
            try
            {
                var createCartDto = await _cartService.CreateCartDtoAsync(userId);
                return Ok(createCartDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("User not found"))
            {
                _logger.LogWarning(ex, "User {UserId} not found when creating cart", userId);
                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                return StatusCode(500, $"Error creating cart: {ex.Message}");
            }
        }

        [HttpPost("{cartId}/add")]
        public async Task<ActionResult<CartDto>> AddItem(Guid cartId, [FromBody] AddItemDto dto)
        {
            _logger.LogInformation("AddItem called for Cart {CartId} Product {ProductId} Quantity {Quantity}", cartId, dto.ProductId, dto.Quantity);
            try
            {
                var cartDto = await _cartService.AddItemDtoAsync(cartId, dto.ProductId, dto.Quantity);

                if (cartDto == null)
                {
                    _logger.LogWarning("AddItem returned null for cart {CartId}", cartId);
                    return NotFound("Cart not found or product not found");
                }

                return Ok(cartDto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Cart not found"))
            {
                _logger.LogWarning(ex, "Cart {CartId} not found when adding item", cartId);
                return NotFound("Cart not found");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Product not found"))
            {
                _logger.LogWarning(ex, "Product {ProductId} not found when adding to cart {CartId}", dto.ProductId, cartId);
                return NotFound("Product not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart {CartId}", cartId);
                return StatusCode(500, $"Error adding item: {ex.Message}");
            }
        }

        [HttpDelete("{cartId}/remove/{productId}")]
        public async Task<ActionResult<CartDto>> RemoveItem(Guid cartId, Guid productId)
        {
            _logger.LogInformation("RemoveItem called for Cart {CartId} Product {ProductId}", cartId, productId);
            try
            {
                var success = await _cartService.RemoveItemAsync(cartId, productId);

                if (!success)
                {
                    _logger.LogWarning("RemoveItem failed for Cart {CartId} Product {ProductId}", cartId, productId);
                    return NotFound("Cart or item not found");
                }

                var cartDto = await _cartService.GetCartDtoAsync(cartId);
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ProductId} from cart {CartId}", productId, cartId);
                return StatusCode(500, "Error removing item from cart");
            }
        }

        [HttpDelete("{cartId}/clear")]
        public async Task<ActionResult<CartDto>> ClearCart(Guid cartId)
        {
            _logger.LogInformation("ClearCart called for Cart {CartId}", cartId);
            try
            {
                var success = await _cartService.ClearCartAsync(cartId);

                if (!success)
                {
                    _logger.LogWarning("ClearCart failed for Cart {CartId}", cartId);
                    return NotFound("Cart not found");
                }

                var cartDto = await _cartService.GetCartDtoAsync(cartId);
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart {CartId}", cartId);
                return StatusCode(500, "Error clearing cart");
            }
        }
    }
}
