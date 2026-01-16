using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain.ValueObjects;
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
        var entity = await _chainRepository.GetByIdAsync(command.Id);
        if (entity == null)
        {
            return Result.Fail(Errors.General.NotFound<ChainId>(command.Id));
        }
        if (_chainRepository.GetCountofStoresByChainAsync(command.Id).Result > 0)
        {
            return Result.Fail(Errors.ChainErrors.ChainHasStores());
        }
        await _chainRepository.DeleteAsync(command.Id, cancellationToken);

        return Result.Ok();
    }
}
