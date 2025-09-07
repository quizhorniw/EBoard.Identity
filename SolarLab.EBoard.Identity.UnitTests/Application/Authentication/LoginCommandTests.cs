using System.Security.Authentication;
using Moq;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Login;
using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Authentication;

public class LoginCommandTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IRefreshTokensRepository> _refreshTokensRepositoryMock;
    private readonly Mock<ICookieContext> _cookieContextMock;
    private readonly LoginHandler _handler;
    
    private readonly DateTime _fixedUtcNow;
    private readonly User _testUser;
    
    public LoginCommandTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _refreshTokensRepositoryMock = new Mock<IRefreshTokensRepository>();
        _cookieContextMock = new Mock<ICookieContext>();
        _handler = new LoginHandler(
            _usersRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenProviderMock.Object,
            _cookieContextMock.Object,
            _refreshTokensRepositoryMock.Object
            );

        _fixedUtcNow = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _testUser = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");
    }

    [Fact]
    public async Task HandleValidRequest_ReturnsAccessToken()
    {
        // Arrange
        _usersRepositoryMock.Setup(r => r
                .GetByEmailAsync(_testUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUser);
        
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), "hash")).Returns(true);

        const string accessToken = "accessToken";
        _tokenProviderMock.Setup(p => p.CreateAccessToken(_testUser)).Returns(accessToken);
        
        _tokenProviderMock.Setup(p => p
                .GenerateRefreshToken(_testUser.Id))
            .Returns(new RefreshToken(_testUser.Id, accessToken, _fixedUtcNow, _fixedUtcNow.AddDays(1)));
        
        // Act
        var result = await _handler.Handle(new LoginCommand(_testUser.Email, "some-password"), CancellationToken.None);

        // Assert
        Equal(accessToken, result.AccessToken);
    }

    [Fact]
    public async Task HandleValidRequest_SavesRefreshToken()
    {
        // Arrange
        _usersRepositoryMock.Setup(r => r
                .GetByEmailAsync(_testUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUser);
        
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), "hash")).Returns(true);

        const string accessToken = "accessToken";
        _tokenProviderMock.Setup(p => p.CreateAccessToken(_testUser)).Returns(accessToken);

        var refreshToken = new RefreshToken(_testUser.Id, accessToken, _fixedUtcNow, _fixedUtcNow.AddDays(1));
        _tokenProviderMock.Setup(p => p
                .GenerateRefreshToken(_testUser.Id))
            .Returns(refreshToken);
        
        // Act
        await _handler.Handle(new LoginCommand(_testUser.Email, "some-password"), CancellationToken.None);

        // Assert
        _refreshTokensRepositoryMock.Verify(r => r
            .AddAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task HandleValidRequest_AddsRefreshTokenCookie()
    {
        // Arrange
        _usersRepositoryMock.Setup(r => r
                .GetByEmailAsync(_testUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUser);
        
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), "hash")).Returns(true);

        const string accessToken = "accessToken";
        _tokenProviderMock.Setup(p => p.CreateAccessToken(_testUser)).Returns(accessToken);

        var refreshToken = new RefreshToken(_testUser.Id, accessToken, _fixedUtcNow, _fixedUtcNow.AddDays(1));
        _tokenProviderMock.Setup(p => p
                .GenerateRefreshToken(_testUser.Id))
            .Returns(refreshToken);
        
        // Act
        await _handler.Handle(new LoginCommand(_testUser.Email, "some-password"), CancellationToken.None);

        // Assert
        _cookieContextMock.Verify(c => c.AppendRefreshTokenCookie(refreshToken), Times.Once);
    }

    [Fact]
    public async Task HandleRequest_WithInvalidEmail_ThrowsAuthenticationException()
    {
        // Arrange
        const string email = "non-existing-user@mail.com";
        _usersRepositoryMock.Setup(r => r
                .GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        // Assert
        await ThrowsAsync<AuthenticationException>(() => 
            _handler.Handle(new LoginCommand(email, "some-password"), CancellationToken.None));
    }
    
    [Fact]
    public async Task HandleRequest_WithInvalidPassword_ThrowsAuthenticationException()
    {
        // Arrange
        _usersRepositoryMock.Setup(r => r
                .GetByEmailAsync(_testUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testUser);
        
        _passwordHasherMock.Setup(h => h.Verify(It.IsAny<string>(), "hash")).Returns(false);
        
        // Act
        // Assert
        await ThrowsAsync<AuthenticationException>(() => 
            _handler.Handle(new LoginCommand(_testUser.Email, "some-password"), CancellationToken.None));
    }
}