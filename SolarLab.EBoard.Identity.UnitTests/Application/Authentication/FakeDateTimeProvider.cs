using SolarLab.EBoard.Identity.Domain.Commons;

namespace SolarLab.EBoard.Identity.UnitTests.Application.Authentication;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => new DateTime(2000, 1, 1);
}