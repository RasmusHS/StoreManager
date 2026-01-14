using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store.Handlers;

public class CreateStoreCommandHandler : ICommandHandler<CreateStoreCommand>
{
    private readonly IStoreRepository _storeRepository;
    
    public CreateStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result> Handle(CreateStoreCommand command, CancellationToken cancellationToken = default)
    {
        if (command.ChainId == null)
        {
            return Result.Fail(Errors.General.ValueIsRequired(nameof(command.ChainId)));
        }

        //Result<Address> addressResult = Address.Create(command.Address.Street, command.Address.PostalCode, command.Address.City);
        //if (addressResult.Failure) return addressResult;

        //Result<PhoneNumber> phoneResult = PhoneNumber.Create(command.PhoneNumber.CountryCode, command.PhoneNumber.Number);
        //if (phoneResult.Failure) return phoneResult;

        //Result<Email> emailResult = Email.Create(command.Email.Value);
        //if (emailResult.Failure) return emailResult;

        //Result<FullName> storeOwnerResult = FullName.Create(command.StoreOwner.FirstName, command.StoreOwner.LastName);
        //if (storeOwnerResult.Failure) return storeOwnerResult;

        Result<StoreEntity> storeResult = StoreEntity.Create(
            command.ChainId,
            command.Number,
            command.Name,
            command.Address,
            command.PhoneNumber,
            command.Email,
            command.StoreOwner);
        if (storeResult.Failure) return storeResult;

        await _storeRepository.AddAsync(storeResult.Value, cancellationToken);

        return Result.Ok();
    }
}
