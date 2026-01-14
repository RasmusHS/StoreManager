using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Commands.Chain.Handlers;

public class DeleteChainCommandHandler : ICommandHandler<DeleteChainCommand>
{
    private readonly IChainRepository _chainRepository;
    
    public DeleteChainCommandHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result> Handle(DeleteChainCommand command, CancellationToken cancellationToken = default)
    {
        await _chainRepository.DeleteAsync(command.Id, cancellationToken);

        return Result.Ok();
    }
}
