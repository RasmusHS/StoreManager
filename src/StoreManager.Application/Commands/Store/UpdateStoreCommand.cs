using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class UpdateStoreCommand : ICommand<StoreResponseDto>
{
    public UpdateStoreCommand(StoreId id, ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        ChainId = chainId;
        Number = number;
        Name = name;
        
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public UpdateStoreCommand() { }

    public StoreId Id { get; set; }
    public ChainId? ChainId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public Email Email { get; set; }
    public FullName StoreOwner { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
