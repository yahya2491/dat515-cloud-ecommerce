using Ecommerce.Api.Services;

namespace Ecommerce.Api.Services
{
    public interface IAnalyticsService
    {
        void LogProductView(Guid productId);
        void LogProductAddedToCart(Guid productId);
        void LogProductPurchase(Guid productId);
    }
}
