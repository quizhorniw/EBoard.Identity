using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void WhenCreateUser_WithValidData_UserIsCreated()
    {
        // Arrange
        const string email = "test@mail.com";
        const string phoneNumber = "+79180576819";
        const string firstName = "Иван";
        const string lastName = "Иванов";
        const string passwordHash = "hash";
        
        // Act
        var user = new User(email, phoneNumber, firstName, lastName, passwordHash);
        
        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal("User", user.Role);
    }

    [Fact]
    public void WhenUserIsCreated_RaisesUserRegisteredDomainEvent()
    {
        // Act
        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");

        // Assert
        Assert.Single(user.DomainEvents);
        var @event = Assert.IsType<UserRegisteredDomainEvent>(user.DomainEvents.First());
        Assert.Equal(user.Id, @event.UserId);    
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("invalid@")]
    public void WhenCreateUser_WithInvalidEmail_ThrowsArgumentException(string email)
    {
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => 
            new User(email, "+79180576819", "Иван", "Иванов", "hash"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("some-letters")]
    [InlineData("12345")]
    [InlineData("+12345")]
    public void When_CreateUser_WithInvalidPhoneNumber_ThrowsArgumentException(string phoneNumber)
    {
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => 
            new User("test@mail.com", phoneNumber, "Иван", "Иванов", "hash"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text with whitespaces\r\n")]
    public void When_CreateUser_WithInvalidFirstName_ThrowsArgumentException(string firstName)
    {
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => 
            new User("test@mail.com", "+79180576819", firstName, "Иванов", "hash"));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text with whitespaces\r\n")]
    public void When_CreateUser_WithInvalidLastName_ThrowsArgumentException(string lastName)
    {
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => 
            new User("test@mail.com", "+79180576819", "Иван", lastName, "hash"));
    }
}