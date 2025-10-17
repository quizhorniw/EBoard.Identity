using System.Text.RegularExpressions;
using SolarLab.EBoard.Identity.Domain.Commons;

namespace SolarLab.EBoard.Identity.Domain.Entities;

public sealed class User : Entity
{
    private readonly Regex _emailRegex = new(@"^\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly Regex _phoneNumberRegex = new(@"^\+7\d{10}$", RegexOptions.Compiled);
    
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public string? ConfirmationToken { get; private set; }
    public bool IsConfirmed { get; private set; }

    public User(string email, string? phoneNumber, string firstName, string lastName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email) || !_emailRegex.IsMatch(email))
        {
            throw new ArgumentException("Invalid email address.", nameof(email));
        }

        if (phoneNumber != null && !_phoneNumberRegex.IsMatch(phoneNumber))
        {
            throw new ArgumentException("Invalid phone number.", nameof(phoneNumber));
        }

        if (string.IsNullOrWhiteSpace(firstName) || firstName.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Invalid first name.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("Invalid last name.", nameof(lastName));
        }
        
        Id = Guid.NewGuid();
        Email = email;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Role = "User";
        IsConfirmed = false;
        
        Raise(new UserRegisteredDomainEvent(Id));
    }

    public void SetConfirmationToken(string confirmationToken)
    {
        ConfirmationToken = confirmationToken;
    }

    public void ConfirmEmail() => IsConfirmed = true;
}