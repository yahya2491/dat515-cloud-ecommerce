using Ecommerce.Api.Models.Dtos;
using FluentValidation;

namespace Ecommerce.Api.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Category).MaximumLength(100).When(x => x.Category != null);
        }
    }

    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Category).MaximumLength(100).When(x => x.Category != null);
        }
    }
}
