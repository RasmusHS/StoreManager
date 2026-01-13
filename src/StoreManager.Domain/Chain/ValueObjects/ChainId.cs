using StoreManager.Domain.Common;

namespace StoreManager.Domain.Chain.ValueObjects;

public sealed class ChainId : ValueObject
{
    public Guid Value { get; }

    private ChainId(Guid value)
    {
        Value = value;
    }

    public static Result<ChainId> Create()
    {
        return Result.Ok<ChainId>(new ChainId(Guid.NewGuid()));
    }

    public static Result<ChainId> GetExisting(Guid value)
    {
        return Result.Ok<ChainId>(new ChainId(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
