using SolarLab.EBoard.Identity.Domain.Commons;

namespace SolarLab.EBoard.Identity.Infrastructure.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}