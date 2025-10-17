namespace SolarLab.EBoard.Identity.Application.Abstractions.Messaging;

public interface IMessageProducer
{
    Task SendAsync(object message, CancellationToken cancellationToken = default);
}