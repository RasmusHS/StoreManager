using MediatR;
using StoreManager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManager.Application.Data;

public interface IQueryHandler<in TQuery, TResult>
    : IRequestHandler<TQuery, Result<TResult>> where TQuery : IQuery<TResult>
{
    new Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
