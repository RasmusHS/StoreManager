using StoreManager.Domain.Common;

namespace StoreManager.Domain.Store.ValueObjects;

public sealed class StoreId : ValueObject
{
    public Guid Value { get; }

    private StoreId(Guid value)
    {
        Value = value;
    }

    public static Result<StoreId> Create()
    {
        return Result.Ok<StoreId>(new StoreId(Guid.NewGuid()));
    }

    public static Result<StoreId> GetExisting(Guid value)
    {
        return Result.Ok<StoreId>(new StoreId(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
