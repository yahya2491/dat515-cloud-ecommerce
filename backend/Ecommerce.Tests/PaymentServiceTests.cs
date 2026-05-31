using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Ecommerce.Tests.Fakes;
using Ecommerce.Tests.TestHelpers;

namespace Ecommerce.Tests
{
    public class PaymentServiceTests
    {
        [Fact]
        public async Task ProcessPaymentAsync_ReturnsFailure_WhenUserNotFound()
        {
            var db = InMemoryDbFactory.Create("Payment_NoUser");
            var fakeUserSvc = new FakeUserService(null);
            var svc = new PaymentService(db, fakeUserSvc, Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentService>.Instance);

            var result = await svc.ProcessPaymentAsync(new PaymentRequest { UserId = Guid.NewGuid(), Amount = 100, Currency = "usd" });

            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        [Fact]
        public async Task ProcessPaymentAsync_HandlesUserServiceException()
        {
            var db = InMemoryDbFactory.Create("Payment_UserSvcThrows");
            var fakeUserSvc = new FakeUserService(throwOnGet: true);
            var svc = new PaymentService(db, fakeUserSvc, Microsoft.Extensions.Logging.Abstractions.NullLogger<PaymentService>.Instance);

            var result = await svc.ProcessPaymentAsync(new PaymentRequest { UserId = Guid.NewGuid(), Amount = 50, Currency = "usd" });

            Assert.False(result.Success);
            Assert.Contains("Unexpected error", result.Message);
        }
    }
}
