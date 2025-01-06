using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceWebApiDto.Validators
{
    public class OrderRequestDtoValidator : AbstractValidator<OrderRequestDto>
    {
        public OrderRequestDtoValidator()
        {
            RuleFor(orderRequestDto => orderRequestDto.CustomerId)
             .GreaterThan(0)
             .WithMessage("CustomerId is required and must be greater than 0.");

            RuleFor(orderRequestDto => orderRequestDto.Products)
                .NotNull()
                .WithMessage("Products cannot be null.")
                .NotEmpty()
                .WithMessage("Products must contain at least one item.");

            RuleForEach(order => order.Products).SetValidator(new ProductRequestDtoValidator());
        }
    }
}
