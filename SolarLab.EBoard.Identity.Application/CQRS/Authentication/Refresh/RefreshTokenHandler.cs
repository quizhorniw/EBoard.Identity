using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Domain.Commons;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;

internal sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly ICookieContext _cookieContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenHandler(
        IRefreshTokensRepository refreshTokensRepository, 
        ITokenProvider tokenProvider, ICookieContext cookieContext,
        IUsersRepository usersRepository, 
        IDateTimeProvider dateTimeProvider)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _usersRepository = usersRepository;
        _dateTimeProvider = dateTimeProvider;
        _tokenProvider = tokenProvider;
        _cookieContext = cookieContext;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldToken = await _refreshTokensRepository.GetByTokenAsync(request.OldToken, cancellationToken);
        if (oldToken is null || !oldToken.IsActive(_dateTimeProvider.UtcNow))
        {
            throw new UnauthorizedAccessException();
        }
        
        // Token replacement
        var newToken = _tokenProvider.GenerateRefreshToken(oldToken.UserId);
        oldToken.Revoke(_dateTimeProvider.UtcNow);
        
        await _refreshTokensRepository.AddAsync(newToken, cancellationToken);
        
        var user = await _usersRepository.GetByIdAsync(oldToken.UserId, cancellationToken);
        var newAccess = _tokenProvider.CreateAccessToken(user!);
        
        _cookieContext.AppendRefreshTokenCookie(newToken);
        
        return new RefreshTokenResponse(newAccess);
    }
}