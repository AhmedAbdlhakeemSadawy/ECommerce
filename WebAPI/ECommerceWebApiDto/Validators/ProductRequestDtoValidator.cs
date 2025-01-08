using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceWebApiDto.Validators
{
    public class ProductRequestDtoValidator : AbstractValidator<ProductRequestDto>
    {
        public ProductRequestDtoValidator()
        {
            RuleFor(productRequestDto => productRequestDto.Id)
            .GreaterThan(0)
            .WithMessage("ProductId is required.");

            RuleFor(productRequestDto => productRequestDto.Quantity)
            .GreaterThan(0)
            .WithMessage("Product quantity at least should be one");
        }
    }
}
