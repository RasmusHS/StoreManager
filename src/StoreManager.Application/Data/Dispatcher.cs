using EnsureThat;
using MediatR;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Data;

public class Dispatcher : IDispatcher
{
    public Dispatcher(IMediator mediator)
    {
        Ensure.That(mediator).IsNotNull();
        Mediator = mediator;
    }

    private IMediator Mediator { get; }

    public Task<Result<T>> Dispatch<T>(IQuery<T> query)
    {
        Ensure.That(query, nameof(query)).IsNotNull();

        return Mediator.Send(query);
    }

    public Task<Result> Dispatch(ICommand command)
    {
        Ensure.That(command, nameof(command)).IsNotNull();

        return Mediator.Send(command);
    }
}
