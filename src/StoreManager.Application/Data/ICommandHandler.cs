using MediatR;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Data;

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand, Result> where TCommand : ICommand
{
    new Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResult>
    : IRequestHandler<TCommand, Result<TResult>> where TCommand : ICommand<TResult>
{
    new Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
}