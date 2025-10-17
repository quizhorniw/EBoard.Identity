using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;

public sealed record ConfirmEmailCommand(Guid UserId, string ConfirmationToken) : IRequest;