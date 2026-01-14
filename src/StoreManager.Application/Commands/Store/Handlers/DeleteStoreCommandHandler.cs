using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Application.Commands.Store.Handlers;

public class DeleteStoreCommandHandler : ICommandHandler<DeleteStoreCommand>
{
    private readonly IStoreRepository _storeRepository;
    
    public DeleteStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result> Handle(DeleteStoreCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Id != null && command.ChainId == null)
        {
            await _storeRepository.DeleteAsync(command.Id, cancellationToken);
            return Result.Ok();
        }
        else if (command.Id == null && command.ChainId != null)
        {
            await _storeRepository.DeleteByChainIdAsync(command.ChainId, cancellationToken);
            return Result.Ok();
        }
        else
        {
            return Result.Fail(Errors.General.ValueIsRequired($"StoreId or ChainId is required. {command.Id} and {command.ChainId} were provided."));
        }
    }
}
