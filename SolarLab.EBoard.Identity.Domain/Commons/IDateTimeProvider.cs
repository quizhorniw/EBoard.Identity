namespace SolarLab.EBoard.Identity.Domain.Commons;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}