using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Logout;

public sealed class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly ICookieContext _cookieContext;

    public LogoutHandler(ICookieContext cookieContext)
    {
        _cookieContext = cookieContext;
    }

    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _cookieContext.DeleteRefreshTokenCookie();
        return Task.CompletedTask;
    }
}