using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Commands.Chain.Handlers;

public class CreateChainCommandHandler : ICommandHandler<CreateChainCommand, ChainResponseDto>
{
    private readonly IChainRepository _chainRepository;

    public CreateChainCommandHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result<ChainResponseDto>> Handle(CreateChainCommand command, CancellationToken cancellationToken = default)
    {
        List<Error> errors = new List<Error>();
        try
        {
            if (command.Stores != null && command.Stores.Count() >= 1)
            {
                Result<ChainEntity> chainResult = ChainEntity.Create(command.Name);
                if (chainResult.Failure) 
                    errors.Add(Errors.General.CreateEntityFailed(nameof(chainResult)));


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
                    if (storeResult.Failure)
                        errors.Add(Errors.General.CreateEntityFailed(nameof(storeResult)));
                    else
                        stores.Add(storeResult.Value);
                }
                if (errors.Count > 0)
                    return Result.Fail<ChainResponseDto>(errors);

                chainResult.Value.AddRangeStoresToChain(stores);
                await _chainRepository.AddAsync(chainResult.Value, cancellationToken);

                var responseDto = new ChainResponseDto
                {
                    Id = chainResult.Value.Id.Value,
                    Name = chainResult.Value.Name,
                    Stores = stores.Select(s => new StoreResponseDto
                    {
                        Id = s.Id.Value,
                        ChainId = s.ChainId?.Value,
                        Number = s.Number,
                        Name = s.Name,
                        Street = s.Address.Street,
                        PostalCode = s.Address.PostalCode,
                        City = s.Address.City,
                        CountryCode = s.PhoneNumber.CountryCode,
                        PhoneNumber = s.PhoneNumber.Number,
                        Email = s.Email.Value,
                        FirstName = s.StoreOwner.FirstName,
                        LastName = s.StoreOwner.LastName,
                        CreatedOn = s.CreatedOn,
                        ModifiedOn = s.ModifiedOn
                    }).ToList(),
                    CreatedOn = chainResult.Value.CreatedOn,
                    ModifiedOn = chainResult.Value.ModifiedOn
                };

                return Result.Ok<ChainResponseDto>(responseDto);
            }
            else
            {
                Result<ChainEntity> chainResult = ChainEntity.Create(command.Name);
                if (chainResult.Failure) return Result.Fail<ChainResponseDto>(Errors.General.CreateEntityFailed(nameof(chainResult)));

                await _chainRepository.AddAsync(chainResult.Value, cancellationToken);

                var responseDto = new ChainResponseDto
                {
                    Id = chainResult.Value.Id.Value,
                    Name = chainResult.Value.Name,
                    CreatedOn = chainResult.Value.CreatedOn,
                    ModifiedOn = chainResult.Value.ModifiedOn
                };

                return Result.Ok<ChainResponseDto>(responseDto);
            } 
        }
        catch (ArgumentException ex)
        {
            return Result.Fail<ChainResponseDto>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
