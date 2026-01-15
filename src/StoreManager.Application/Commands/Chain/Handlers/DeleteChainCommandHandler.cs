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
        if (_chainRepository.GetCountofStoresByChainAsync(command.Id).Result > 0)
        {
            return Result.Fail(Errors.ChainErrors.ChainHasStores());
        }
        await _chainRepository.DeleteAsync(command.Id, cancellationToken);

        return Result.Ok();
    }
}
