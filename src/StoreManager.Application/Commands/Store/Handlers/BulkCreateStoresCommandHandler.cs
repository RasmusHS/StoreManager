using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Commands.Store.Handlers;

public class BulkCreateStoresCommandHandler : ICommandHandler<BulkCreateStoresCommand, CollectionResponseBase<StoreResponseDto>>
{
    private readonly IStoreRepository _storeRepository;

    public BulkCreateStoresCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result<CollectionResponseBase<StoreResponseDto>>> Handle(BulkCreateStoresCommand command, CancellationToken cancellationToken = default)
    {
        List<StoreEntity> stores = new List<StoreEntity>();
        List<Error> errors = new List<Error>();
        try
        {
            for (var i = 0; i < command.Stores.Count; i++)
            {
                Result<StoreEntity> store = StoreEntity.Create(
                    command.Stores[i].ChainId,
                    command.Stores[i].Number,
                    command.Stores[i].Name,
                    command.Stores[i].Address,
                    command.Stores[i].PhoneNumber,
                    command.Stores[i].Email,
                    command.Stores[i].StoreOwner);
                if (store.Failure) 
                    errors.Add(Errors.General.CreateEntityFailed(store.Error));
                else
                    stores.Add(store.Value);
            }

            if (errors.Any())
            {
                return Result.Fail<CollectionResponseBase<StoreResponseDto>>(errors);
            }

            var createdStores = await _storeRepository.AddRangeAsync(stores, cancellationToken);
            var storeDtos = createdStores.Select(s => new StoreResponseDto()
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
            }).ToList();

            return new CollectionResponseBase<StoreResponseDto>()
            { 
                Data = storeDtos 
            };
        }
        catch (Exception ex)
        {
            return Result.Fail<CollectionResponseBase<StoreResponseDto>>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
