using SolarLab.EBoard.Identity.Domain.Entities;
using static Xunit.Assert;

namespace SolarLab.EBoard.Identity.UnitTests.Domain;

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
        NotEqual(Guid.Empty, user.Id);
        Equal(email, user.Email);
        Equal("User", user.Role);
    }

    [Fact]
    public void WhenUserCreated_RaisesUserRegisteredDomainEvent()
    {
        // Act
        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");

        // Assert
        Single(user.DomainEvents);
        var @event = IsType<UserRegisteredDomainEvent>(user.DomainEvents.First());
        Equal(user.Id, @event.UserId);    
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r\n")]
    [InlineData("not-an-email")]
    [InlineData("invalid@")]
    public void WhenCreateUser_WithInvalidEmail_ThrowsArgumentException(string email)
    {
        // Act
        // Assert
        Throws<ArgumentException>(() => 
            new User(email, "+79180576819", "Иван", "Иванов", "hash"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("\r\n")]
    [InlineData("some-letters")]
    [InlineData("12345")]
    [InlineData("+12345")]
    public void WhenCreateUser_WithInvalidPhoneNumber_ThrowsArgumentException(string phoneNumber)
    {
        // Act
        // Assert
        Throws<ArgumentException>(() => 
            new User("test@mail.com", phoneNumber, "Иван", "Иванов", "hash"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r\n")]
    [InlineData("text with whitespaces\r\n")]
    public void WhenCreateUser_WithInvalidFirstName_ThrowsArgumentException(string firstName)
    {
        // Act
        // Assert
        Throws<ArgumentException>(() => 
            new User("test@mail.com", "+79180576819", firstName, "Иванов", "hash"));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r\n")]
    [InlineData("text with whitespaces\r\n")]
    public void WhenCreateUser_WithInvalidLastName_ThrowsArgumentException(string lastName)
    {
        // Act
        // Assert
        Throws<ArgumentException>(() => 
            new User("test@mail.com", "+79180576819", "Иван", lastName, "hash"));
    }

    [Theory]
    [InlineData("confirmation token")]
    [InlineData("SoMe Token 2")]
    [InlineData("3-4-5-6-7-8")]
    public void SetEmailConfirmationToken_SetsEmailConfirmationToken(string confirmationToken)
    {
        // Arrange
        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");
        
        // Act
        user.SetConfirmationToken(confirmationToken);
        
        // Assert
        Equal(confirmationToken, user.ConfirmationToken);
    }

    [Fact]
    public void ConfirmEmail_ConfirmsEmail()
    {
        // Arrange
        var user = new User("test@mail.com", "+79180576819", "Иван", "Иванов", "hash");
        
        // Act
        user.ConfirmEmail();
        
        // Assert
        True(user.IsConfirmed);
    }
}