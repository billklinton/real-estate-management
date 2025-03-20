using FluentValidation;

namespace RealEstateManagement.Shareable.Requests.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(request => request.Email)
                .EmailAddress()
                .NotEmpty()
                .WithMessage("Email can't be empty or invalid");

            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage("Password can't be empty");
        }
    }
}
