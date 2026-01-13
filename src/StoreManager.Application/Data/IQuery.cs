using MediatR;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Data;

public interface IQuery<T> : IRequest<Result<T>>
{
}
