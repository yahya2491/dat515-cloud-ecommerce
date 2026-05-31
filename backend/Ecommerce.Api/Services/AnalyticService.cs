using Prometheus;

namespace Ecommerce.Api.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(ILogger<AnalyticsService> logger)
        {
            _logger = logger;
        }
        private static readonly Counter ProductViewCounter =
            Metrics.CreateCounter("product_views_total", "Total number of product views", new[] { "product_id" });

        private static readonly Counter ProductAddToCartCounter =
            Metrics.CreateCounter("product_added_to_cart_total", "Total number of products added to cart", new[] { "product_id" });

        private static readonly Counter ProductPurchaseCounter =
            Metrics.CreateCounter("product_purchased_total", "Total number of products purchased", new[] { "product_id" });

        public void LogProductView(Guid productId)
        {
            try
            {
                ProductViewCounter.WithLabels(productId.ToString()).Inc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing product view counter for {ProductId}", productId);
            }
        }

        public void LogProductAddedToCart(Guid productId)
        {
            try
            {
                ProductAddToCartCounter.WithLabels(productId.ToString()).Inc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing add-to-cart counter for {ProductId}", productId);
            }
        }

        public void LogProductPurchase(Guid productId)
        {
            try
            {
                ProductPurchaseCounter.WithLabels(productId.ToString()).Inc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing purchase counter for {ProductId}", productId);
            }
        }
    }
}
