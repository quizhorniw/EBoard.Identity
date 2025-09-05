using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Domain.Entities;

public class RefreshTokenTests
{
    private readonly DateTime _fixedUtcNow = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    [Fact]
    public void WhenCreateRefreshToken_WithValidData_RefreshTokenIsCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string token = "token";
        var createdAt = _fixedUtcNow;
        var expiresAt = createdAt.AddDays(1);
        
        // Act
        var refreshToken = new RefreshToken(userId, token, createdAt, expiresAt);
        
        // Assert
        NotEqual(Guid.Empty, refreshToken.Id);
        Equal(token, refreshToken.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r\n")]
    public void WhenCreateRefreshToken_WithInvalidToken_ThrowsArgumentException(string token)
    {
        // Act
        // Assert
        Throws<ArgumentException>(() => 
            new RefreshToken(Guid.NewGuid(), token, _fixedUtcNow, _fixedUtcNow.AddDays(1)));
    }
    
    [Fact]
    public void WhenCreateRefreshToken_WithExpiredExpiration_RefreshTokenIsNotActive()
    {
        // Arrange
        var createdAt = _fixedUtcNow;
        var expiresAt = createdAt.AddDays(-1);
        
        // Act
        // Assert
        Throws<ArgumentException>(() => new RefreshToken(Guid.NewGuid(), "token", createdAt, expiresAt));
    }
    
    [Fact]
    public void WhenCreateRefreshToken_WithExpirationLaterNow_RefreshTokenIsActive()
    {
        // Arrange
        var createdAt = _fixedUtcNow;
        var expiresAt = createdAt.AddDays(1);
        
        // Act
        var refreshToken = new RefreshToken(Guid.NewGuid(), "token", createdAt, expiresAt);
        
        // Assert
        True(refreshToken.IsActive(createdAt));
    }

    [Fact]
    public void WhenRevokeRefreshToken_RefreshTokenIsNotActive()
    {
        // Arrange
        var createdAt = _fixedUtcNow;
        var refreshToken = new RefreshToken(Guid.NewGuid(), "token", createdAt, createdAt.AddDays(1));

        // Act
        refreshToken.Revoke(createdAt.AddMinutes(1));
        
        // Assert
        False(refreshToken.IsActive(createdAt.AddMinutes(2)));
        Equal(createdAt.AddMinutes(1), refreshToken.Revoked);
    }
}