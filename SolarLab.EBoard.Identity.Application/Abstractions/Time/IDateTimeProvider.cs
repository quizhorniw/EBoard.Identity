namespace SolarLab.EBoard.Identity.Application.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}