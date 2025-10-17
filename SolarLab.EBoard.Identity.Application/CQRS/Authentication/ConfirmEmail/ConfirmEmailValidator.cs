using FluentValidation;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;

internal sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ConfirmationToken).NotEmpty();
    }
}