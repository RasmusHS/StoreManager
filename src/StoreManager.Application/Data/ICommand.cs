using MediatR;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Data;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResult> : IRequest<Result<TResult>>
{
}
