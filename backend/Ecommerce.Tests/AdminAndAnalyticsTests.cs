using Ecommerce.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.Tests
{
    public class AdminAndAnalyticsTests
    {
        [Fact]
        public void AnalyticsMethods_DoNotThrow()
        {
            var analytics = new AnalyticsService(NullLogger<AnalyticsService>.Instance);
            var id = Guid.NewGuid();

            analytics.LogProductView(id);
            analytics.LogProductAddedToCart(id);
            analytics.LogProductPurchase(id);
        }
    }
}
