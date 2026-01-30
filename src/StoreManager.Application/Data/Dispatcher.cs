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

    public async Task<Result> Dispatch(ICommand command, CancellationToken cancellationToken = default)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    public async Task<Result<TResult>> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    public async Task<Result<TResult>> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        return await Mediator.Send(query, cancellationToken);
    }

    //public async Task<Result<TResult>> Dispatch<TResult>(IList<ICommand<TResult>> command, CancellationToken cancellationToken = default)
    //{
    //    return await Mediator.Send(command, cancellationToken);
    //}
}
