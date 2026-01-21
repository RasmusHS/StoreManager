using StoreManager.Domain.Common;

namespace StoreManager.Application.Data;

public interface IDispatcher
{
    Task<Result<TResult>> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    Task<Result> Dispatch(ICommand command, CancellationToken cancellationToken = default);
    Task<Result<TResult>> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    // Method to dispatch multiple commands in a single transaction
    //Task<Result<TResult>> Dispatch<TResult>();
}
