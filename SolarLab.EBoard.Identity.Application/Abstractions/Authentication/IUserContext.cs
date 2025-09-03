namespace SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    
    bool IsInRole(string role);
}