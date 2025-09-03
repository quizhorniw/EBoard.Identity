using MediatR;

namespace SolarLab.EBoard.Identity.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<string>;