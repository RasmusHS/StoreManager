using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Commands.Store.Handlers;

public class DeleteAllStoresCommandHandler : ICommandHandler<DeleteAllStoresCommand>
{
    private readonly IChainRepository _chainRepository;
    private readonly IStoreRepository _storeRepository;
    
    public DeleteAllStoresCommandHandler(IChainRepository chainRepository, IStoreRepository storeRepository)
    {
        _chainRepository = chainRepository;
        _storeRepository = storeRepository;
    }
    
    public async Task<Result> Handle(DeleteAllStoresCommand command, CancellationToken cancellationToken = default)
    {
        if (_chainRepository.GetByIdAsync(command.ChainId).Result == null)
        {
            return Result.Fail(Errors.General.NotFound<ChainId>(command.ChainId));
        }
        await _storeRepository.DeleteByChainIdAsync(command.ChainId, cancellationToken);

        return Result.Ok();
    }
}
