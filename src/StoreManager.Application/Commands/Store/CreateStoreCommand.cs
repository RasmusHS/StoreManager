using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class CreateStoreCommand : ICommand<StoreResponseDto>
{
    public CreateStoreCommand(ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner)
    {
        ChainId = chainId;
        Number = number;
        Name = name;

        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;
    }

    public CreateStoreCommand() { }

    public ChainId? ChainId { get; set; }
    public int Number { get; }
    public string Name { get; }
    public Address Address { get; }
    public PhoneNumber PhoneNumber { get; }
    public Email Email { get; }
    public FullName StoreOwner { get; }
}
