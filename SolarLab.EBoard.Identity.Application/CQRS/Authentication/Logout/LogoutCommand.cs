using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Logout;

public record LogoutCommand : IRequest;