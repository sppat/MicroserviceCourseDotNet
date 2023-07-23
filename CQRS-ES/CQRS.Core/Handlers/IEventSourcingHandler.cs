using CQRS.Core.Domain;

namespace CQRS.Core.Handlers;

public interface IEventSourcingHandler<T> where T : AggregateRoot
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<T> GetByIdAsync(Guid aggregateId);
}