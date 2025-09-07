using Moq;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.CQRS.Authentication.Register;
using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Authentication;

public class RegisterCommandTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterHandler _handler;

    public RegisterCommandTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new RegisterHandler(_repositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task HandleValidRequest_ReturnsRegisteredUserId()
    {
        // Arrange
        var request = new RegisterCommand(
            "test@mail.com",
            "+79180576819",
            "Иван",
            "Иванов",
            "some-password"
        );
        _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns("hash");
        
        // Act
        var id = await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task HandleValidRequest_SavesRegisteredUser()
    {
        // Arrange
        var request = new RegisterCommand(
            "test@mail.com",
            "+79180576819",
            "Иван",
            "Иванов",
            "some-password"
        );
        _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns("hash");

        User? capturedUser = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u)
            .Returns(Task.CompletedTask);
        
        // Act
        await _handler.Handle(request, CancellationToken.None);
        
        // Assert
        NotNull(capturedUser);
        _repositoryMock.Verify(r =>
            r.AddAsync(It.Is<User>(u =>
                u.Email == request.Email &&
                u.PhoneNumber == request.PhoneNumber &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName &&
                u.PasswordHash == "hash"
            ), It.IsAny<CancellationToken>()), Times.Once);
    }
}