using Ecommerce.Api.Services;
using Xunit;

namespace Ecommerce.Tests
{
    public class CategoryPredictorTests
    {
        [Theory]
        [InlineData("Whole Milk", "1 Gallon whole milk.", "Dairy")]
        [InlineData("Bananas", "Fresh yellow bananas.", "Produce")]
        [InlineData("Potato Chips", "Crispy salted potato chips.", "Snacks")]
        public void Predict_ReturnsExpectedCategory(string name, string description, string expected)
        {
            var actual = CategoryPredictor.Predict(name, description);
            Assert.Equal(expected, actual);
        }
    }
}
