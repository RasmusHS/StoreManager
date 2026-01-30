using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store.Handlers;

public class UpdateStoreCommandHandler : ICommandHandler<UpdateStoreCommand, StoreResponseDto>
{
    private readonly IStoreRepository _storeRepository;
    
    public UpdateStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result<StoreResponseDto>> Handle(UpdateStoreCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var storeResult = await _storeRepository.GetByIdAsync(command.Id);
            if (storeResult == null)
            {
                return Result.Fail<StoreResponseDto>(Errors.General.NotFound<StoreId>(command.Id));
            }

            if (command.ChainId == null && storeResult.ChainId == null)
            {
                storeResult.UpdateStore(
                null,
                command.Number,
                command.Name,
                command.Address,
                command.PhoneNumber,
                command.Email,
                command.StoreOwner);

                await _storeRepository.UpdateAsync(storeResult, cancellationToken);
            }
            else if (command.ChainId == null && storeResult.ChainId != null)
            {
                storeResult.UpdateStore(
                null,
                command.Number,
                command.Name,
                command.Address,
                command.PhoneNumber,
                command.Email,
                command.StoreOwner);

                await _storeRepository.UpdateAsync(storeResult, cancellationToken);
            }
            else
            {
                storeResult.UpdateStore(
                command.ChainId,
                command.Number,
                command.Name,
                command.Address,
                command.PhoneNumber,
                command.Email,
                command.StoreOwner);

                await _storeRepository.UpdateAsync(storeResult, cancellationToken);
            }

            var storeResponseDto = new StoreResponseDto
            {
                Id = storeResult.Id.Value,
                ChainId = storeResult.ChainId?.Value,
                Number = storeResult.Number,
                Name = storeResult.Name,
                Street = storeResult.Address.Street,
                PostalCode = storeResult.Address.PostalCode,
                City = storeResult.Address.City,
                CountryCode = storeResult.PhoneNumber.CountryCode,
                PhoneNumber = storeResult.PhoneNumber.Number,
                Email = storeResult.Email.Value,
                FirstName = storeResult.StoreOwner.FirstName,
                LastName = storeResult.StoreOwner.LastName,
                CreatedOn = command.CreatedOn,
                ModifiedOn = command.ModifiedOn
            };

            return Result.Ok<StoreResponseDto>(storeResponseDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<StoreResponseDto>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
