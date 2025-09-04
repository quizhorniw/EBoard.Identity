using FluentValidation;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;

internal sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.FirstName).NotEmpty().MinimumLength(2);
        RuleFor(c => c.LastName).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}