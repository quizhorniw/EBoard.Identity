namespace SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}