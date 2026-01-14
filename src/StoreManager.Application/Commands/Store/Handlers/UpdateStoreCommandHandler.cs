using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Commands.Store.Handlers;

public class UpdateStoreCommandHandler : ICommandHandler<UpdateStoreCommand>
{
    private readonly IStoreRepository _storeRepository;
    
    public UpdateStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result> Handle(UpdateStoreCommand command, CancellationToken cancellationToken = default)
    {
        var storeResult = await _storeRepository.GetByIdAsync(command.Id) ?? throw new KeyNotFoundException($"Store with Id {command.Id} was not found.");

        storeResult.UpdateStore(
            command.ChainId,
            command.Number,
            command.Name,
            command.Address,
            command.PhoneNumber,
            command.Email,
            command.StoreOwner);

        await _storeRepository.UpdateAsync(storeResult, cancellationToken);

        return Result.Ok();
    }
}
