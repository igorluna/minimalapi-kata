using FluentValidation;
using minimal_kata.Models.DTO;

namespace minimal_kata.Validations
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDTO>
    {public CouponUpdateValidation()
    {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        
    }
    }
}