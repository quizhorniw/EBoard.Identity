using Microsoft.Extensions.Logging;
using Moq;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.ConfirmEmail;
using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Authentication;

public class ConfirmEmailTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly ConfirmEmailHandler _handler;

    private static readonly Guid TestUserId = Guid.Parse("c0bdab5f-94c2-4141-a138-35ddb34d4d3e");
    private const string TestConfirmationToken = "Test Confirmation Token";

    public ConfirmEmailTests()
    {
        var loggerMock = new Mock<ILogger<ConfirmEmailHandler>>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        _handler = new ConfirmEmailHandler(loggerMock.Object, _usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ConfirmEmail_ConfirmsEmail()
    {
        // Arrange
        var user = new User("test@mail.com", "+71234567890", "FirstName", "LastName", "hash");
        user.SetConfirmationToken(TestConfirmationToken);
        
        var request = new ConfirmEmailCommand(TestUserId, TestConfirmationToken);

        _usersRepositoryMock
            .Setup(r => r.GetByIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(user.IsConfirmed);
    }
    
    [Fact]
    public async Task ConfirmEmail_UpdatedUserInDatabase()
    {
        // Arrange
        var user = new User("test@mail.com", "+71234567890", "FirstName", "LastName", "hash");
        user.SetConfirmationToken(TestConfirmationToken);
        
        var request = new ConfirmEmailCommand(TestUserId, TestConfirmationToken);

        _usersRepositoryMock
            .Setup(r => r.GetByIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        _usersRepositoryMock.Verify(r => 
                r.UpdateAsync(It.Is<User>(u =>
                        user.Id == u.Id &&
                        user.Email == u.Email && 
                        user.PhoneNumber == u.PhoneNumber &&
                        user.FirstName == u.FirstName &&
                        user.LastName == u.LastName &&
                        user.PasswordHash == u.PasswordHash &&
                        user.ConfirmationToken == u.ConfirmationToken &&
                        u.IsConfirmed), 
                    It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_NotPresentUserInDatabase_Throws()
    {
        // Arrange
        var request = new ConfirmEmailCommand(TestUserId, TestConfirmationToken);
        
        _usersRepositoryMock
            .Setup(r => r.GetByIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        // Act
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }
    
    [Fact]
    public async Task ConfirmEmail_AlreadyConfirmed_DoesNotUpdateUserInDatabase()
    {
        // Arrange
        var user = new User("test@mail.com", "+71234567890", "FirstName", "LastName", "hash");
        user.SetConfirmationToken(TestConfirmationToken);
        user.ConfirmEmail();
        
        var request = new ConfirmEmailCommand(TestUserId, TestConfirmationToken);

        _usersRepositoryMock
            .Setup(r => r.GetByIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        _usersRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task ConfirmEmail_InvalidConfirmationToken_Throws()
    {
        // Arrange
        var user = new User("test@mail.com", "+71234567890", "FirstName", "LastName", "hash");
        user.SetConfirmationToken(TestConfirmationToken);
        
        var request = new ConfirmEmailCommand(TestUserId, "different-token");

        _usersRepositoryMock
            .Setup(r => r.GetByIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
