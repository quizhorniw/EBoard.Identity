using MediatR;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;

public sealed record RefreshTokenCommand(string OldToken) : IRequest<RefreshTokenResponse>;