using SolarLab.EBoard.Identity.Application.Abstractions.Messaging;

namespace SolarLab.EBoard.Identity.IntegrationTests;

public class NoOpMessageProducer : IMessageProducer
{
    public Task SendAsync(object message, CancellationToken cancellationToken = default) => Task.CompletedTask;
}