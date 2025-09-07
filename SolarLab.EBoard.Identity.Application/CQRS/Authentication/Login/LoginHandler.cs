using System.Security.Authentication;
using MediatR;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;

namespace SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly ICookieContext _cookieContext;

    public LoginHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider,
        ICookieContext cookieContext, IRefreshTokensRepository refreshTokensRepository)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _cookieContext = cookieContext;
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new AuthenticationException("No user found");
        }
        
        var verified = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!verified)
        {
            throw new AuthenticationException("Invalid password");
        }
        
        var accessToken = _tokenProvider.CreateAccessToken(user);
        
        var refreshToken = _tokenProvider.GenerateRefreshToken(user.Id);
        _cookieContext.AppendRefreshTokenCookie(refreshToken);
        await _refreshTokensRepository.AddAsync(refreshToken, cancellationToken);
        
        return new LoginResponse(accessToken);
    }
}