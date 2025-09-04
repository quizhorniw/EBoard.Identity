using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;