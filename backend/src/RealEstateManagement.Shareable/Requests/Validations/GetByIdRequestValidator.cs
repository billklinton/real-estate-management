using FluentValidation;

namespace RealEstateManagement.Shareable.Requests.Validations
{
    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage("Id can't be empty");
        }
    }
}
