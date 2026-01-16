using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Common;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Commands.Store.Handlers;

public class CreateStoreCommandHandler : ICommandHandler<CreateStoreCommand, StoreResponseDto>
{
    private readonly IStoreRepository _storeRepository;
    
    public CreateStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result<StoreResponseDto>> Handle(CreateStoreCommand command, CancellationToken cancellationToken = default)
    {
        Result<StoreEntity> storeResult = StoreEntity.Create(
            command.ChainId,
            command.Number,
            command.Name,
            command.Address,
            command.PhoneNumber,
            command.Email,
            command.StoreOwner);
        if (storeResult.Failure) return Result.Fail<StoreResponseDto>(Errors.General.CreateEntityFailed(storeResult));

        await _storeRepository.AddAsync(storeResult.Value, cancellationToken);

        var storeDto = new StoreResponseDto
        {
            Id = storeResult.Value.Id.Value,
            ChainId = storeResult.Value.ChainId?.Value,
            Number = storeResult.Value.Number,
            Name = storeResult.Value.Name,
            Street = storeResult.Value.Address.Street,
            PostalCode = storeResult.Value.Address.PostalCode,
            City = storeResult.Value.Address.City,
            CountryCode = storeResult.Value.PhoneNumber.CountryCode,
            PhoneNumber = storeResult.Value.PhoneNumber.Number,
            Email = storeResult.Value.Email.Value,
            FirstName = storeResult.Value.StoreOwner.FirstName,
            LastName = storeResult.Value.StoreOwner.LastName,
            CreatedOn = storeResult.Value.CreatedOn,
            ModifiedOn = storeResult.Value.ModifiedOn
        };

        return Result.Ok<StoreResponseDto>(storeDto);
    }
}
