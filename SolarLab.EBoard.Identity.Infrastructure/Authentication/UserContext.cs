using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

namespace SolarLab.EBoard.Identity.Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User 
        ?? throw new ApplicationException("User context is unavailable");

    public Guid UserId => User.GetUserId();

    public bool IsInRole(string role) => User.IsInRole(role);
}