using SolarLab.EBoard.Identity.Application.Abstractions.Time;

namespace SolarLab.EBoard.Identity.Infrastructure.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}