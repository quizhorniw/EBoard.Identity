using Moq;
using SolarLab.EBoard.Identity.Application.Abstractions.Persistence;
using SolarLab.EBoard.Identity.Application.CQRS.Users.GetById;
using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Users;

public class GetUserByIdQueryTests
{
    private readonly Mock<IUsersRepository> _repositoryMock;
    private readonly GetUserByIdHandler _handler;
    
    public GetUserByIdQueryTests()
    {
        _repositoryMock = new Mock<IUsersRepository>();
        _handler = new GetUserByIdHandler(_repositoryMock.Object);
    }
    
    [Fact]
    public async Task HandleValidRequest_ReturnsValidUserDto()
    {
        // Arrange
        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");
        var request = new GetUserByIdQuery(user.Id);
        _repositoryMock.Setup(r => r
            .GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        NotNull(result);
        Equal(user.Id, result.Id);
        Equal(user.Email, result.Email);
        Equal(user.PhoneNumber, result.PhoneNumber);
        Equal(user.FirstName, result.FirstName);
        Equal(user.LastName, result.LastName);
    }

    [Fact]
    public async Task HandleRequest_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), CancellationToken.None);
        
        // Assert
        Null(result);
    }
}