using SolarLab.EBoard.Identity.Domain.Entities;

namespace SolarLab.EBoard.Identity.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateToken(User user);
}