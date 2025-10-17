using Moq;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.Abstractions.Time;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Refresh;
using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Authentication;

public class RefreshTokenCommandTests
{
    private readonly Mock<IRefreshTokensRepository> _refreshTokensRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<ICookieContext> _cookieContextMock;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RefreshTokenHandler _handler;

    public RefreshTokenCommandTests()
    {
        _refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _cookieContextMock = new Mock<ICookieContext>();
        _dateTimeProvider = new FakeDateTimeProvider();
        _handler = new RefreshTokenHandler(
            _refreshTokensRepositoryMock.Object,
            _tokenProviderMock.Object,
            _cookieContextMock.Object,
            _usersRepositoryMock.Object,
            _dateTimeProvider
            );
    }

    [Fact]
    public async Task HandleValidRequest_ReturnsNewRefreshToken()
    {
        // Arrange
        ArrangeValidRequest();
        
        // Act
        var refreshToken = await _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None);

        // Assert
        Equal("newAccess", refreshToken.AccessToken);
    }

    [Fact]
    public async Task HandleValidRequest_RevokesOldRefreshToken()
    {
        // Arrange
        var (oldToken, _) = ArrangeValidRequest();
        
        // Act
        await _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None);
        
        // Assert
        False(oldToken.IsActive(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public async Task HandleValidRequest_SavesNewRefreshToken()
    {
        // Arrange
        var (_, newToken) = ArrangeValidRequest();

        // Act
        await _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None);

        // Assert
        _refreshTokensRepositoryMock.Verify(r =>
            r.AddAsync(newToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleValidRequest_AddsRefreshTokenCookie()
    {
        // Arrange
        var (_, newToken) = ArrangeValidRequest();
        
        // Act
        await _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None);

        // Assert
        _cookieContextMock.Verify(c => c.AppendRefreshTokenCookie(newToken), Times.Once);
    }
    
    private (RefreshToken oldToken, RefreshToken newToken) ArrangeValidRequest()
    {
        var oldToken = new RefreshToken(
            Guid.NewGuid(), 
            "oldToken", 
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow.AddDays(1)
        );
        _refreshTokensRepositoryMock.Setup(r => r
                .GetByTokenAsync("oldToken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldToken);

        var newToken = new RefreshToken(
            Guid.NewGuid(), 
            "newToken", 
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow.AddDays(1)
        );
        _tokenProviderMock.Setup(r => r.GenerateRefreshToken(oldToken.UserId)).Returns(newToken);

        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");
        _usersRepositoryMock.Setup(r => r
                .GetByIdAsync(oldToken.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenProviderMock.Setup(p => p.CreateAccessToken(user)).Returns("newAccess");

        return (oldToken, newToken);
    }

    [Fact]
    public async Task HandleRequest_WithNullToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _refreshTokensRepositoryMock.Setup(r => r
                .GetByTokenAsync("oldToken", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        // Assert
        await ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None));
    }

    [Fact]
    public async Task HandleRequest_WithExpiredToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var expiredToken = new RefreshToken(
            Guid.NewGuid(), 
            "oldToken", 
            _dateTimeProvider.UtcNow.AddDays(-2),
            _dateTimeProvider.UtcNow.AddDays(-1)
            );
        _refreshTokensRepositoryMock.Setup(r => r
                .GetByTokenAsync("oldToken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        // Act
        // Assert
        await ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None));
    }
    
    [Fact]
    public async Task HandleRequest_WithRevokedToken_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var oldToken = new RefreshToken(
            Guid.NewGuid(), 
            "oldToken", 
            _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow.AddDays(1)
        );
        oldToken.Revoke(_dateTimeProvider.UtcNow);
        _refreshTokensRepositoryMock.Setup(r => r
                .GetByTokenAsync("oldToken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldToken);

        // Act
        // Assert
        await ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(new RefreshTokenCommand("oldToken"), CancellationToken.None));
    }
}