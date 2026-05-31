using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Api.Attributes;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IAnalyticsService _analytics;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IAnalyticsService analytics, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _analytics = analytics;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            _logger.LogInformation("GetAll products called");
            try
            {
                var products = await _productService.GetAllAsync();
                foreach (var p in products)
                {
                    if (p == null) continue;
                    if (string.IsNullOrWhiteSpace(p.Category))
                        p.Category = "General";
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, "Error retrieving products");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            _logger.LogInformation("GetById product {ProductId} called", id);
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found", id);
                    return NotFound();
                }

                if (product != null && string.IsNullOrWhiteSpace(product.Category))
                    product.Category = "General";

                _analytics.LogProductView(id);

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", id);
                return StatusCode(500, "Error retrieving product");
            }
        }

        [HttpPost]
        [AdminKey]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            _logger.LogInformation("Create product called with name={Name}", product?.Name);
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (product == null)
                    return BadRequest("Product is required.");

                var created = await _productService.CreateAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Error creating product");
            }
        }

        [HttpPut("{id}")]
        [AdminKey]
        public async Task<ActionResult<Product>> Update(Guid id, Product updated)
        {
            _logger.LogInformation("Update product {ProductId} called", id);
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _productService.UpdateAsync(id, updated);
                if (result == null)
                {
                    _logger.LogWarning("Update failed - product {ProductId} not found", id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, "Error updating product");
            }
        }

        [HttpDelete("{id}")]
        [AdminKey]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete product {ProductId} called", id);
            try
            {
                var ok = await _productService.DeleteAsync(id);
                if (!ok)
                {
                    _logger.LogWarning("Delete failed - product {ProductId} not found", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, "Error deleting product");
            }
        }
    }
}
