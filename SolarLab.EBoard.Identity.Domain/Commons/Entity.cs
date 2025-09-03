using MediatR;

namespace SolarLab.EBoard.Identity.Domain.Commons;

public abstract class Entity
{
    private readonly List<INotification> _domainEvents = [];

    public List<INotification> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
