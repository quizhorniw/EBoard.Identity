using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string? PhoneNumber,
    string FirstName, 
    string LastName,
    string Password
    ) : IRequest<Guid>;