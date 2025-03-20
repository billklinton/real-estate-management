using FluentValidation;

namespace RealEstateManagement.Shareable.Requests.Validations
{
    public class GetRealEstateRequestValidator : AbstractValidator<GetRealEstateRequest>
    {
        public GetRealEstateRequestValidator()
        {
            RuleFor(request => request.Page)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Page must be greater than or equal to 0");

            RuleFor(request => request.PageSize)
                .ExclusiveBetween(1, 101)
                .WithMessage("PageSize must be between 1 and 100");
        }
    }
}
