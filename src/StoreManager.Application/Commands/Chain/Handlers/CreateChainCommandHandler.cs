using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Common;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Commands.Chain.Handlers;

public class CreateChainCommandHandler : ICommandHandler<CreateChainCommand>
{
    private readonly IChainRepository _chainRepository;

    public CreateChainCommandHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result> Handle(CreateChainCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Stores != null && command.Stores.Count() >= 1)
        {
            Result<ChainEntity> chainResult = ChainEntity.Create(
                command.Name);
            if (chainResult.Failure) return chainResult;

            List<StoreEntity> stores = new List<StoreEntity>();
            foreach (var store in command.Stores)
            {
                Result<StoreEntity> storeResult = StoreEntity.Create(
                    store.ChainId = chainResult.Value.Id,
                    store.Number,
                    store.Name,
                    store.Address,
                    store.PhoneNumber,
                    store.Email,
                    store.StoreOwner);
                if (storeResult.Failure) return storeResult;
                
                stores.Add(storeResult.Value);
            }
            chainResult.Value.AddRangeStoresToChain(stores);
            await _chainRepository.AddAsync(chainResult.Value, cancellationToken);
        }
        else
        {
            Result<ChainEntity> chainResult = ChainEntity.Create(
                command.Name);
            if (chainResult.Failure) return chainResult;
            await _chainRepository.AddAsync(chainResult.Value, cancellationToken);
        }

        return Result.Ok();
    }
}
