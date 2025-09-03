using MediatR;

namespace SolarLab.EBoard.Identity.Application.Users.Register;

public sealed record RegisterUserCommand(
    string Email,
    string? PhoneNumber,
    string FirstName, 
    string LastName,
    string Password
    ) : IRequest<Guid>;