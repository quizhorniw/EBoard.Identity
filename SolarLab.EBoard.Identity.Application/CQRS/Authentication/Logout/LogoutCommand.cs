using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Logout;

public sealed record LogoutCommand : IRequest;