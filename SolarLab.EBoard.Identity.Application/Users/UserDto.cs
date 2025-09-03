namespace SolarLab.EBoard.Identity.Application.Users;

public sealed record UserDto(
    Guid Id,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName
    );