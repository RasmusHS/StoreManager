using System;
using System.Collections.Generic;
using System.Text;

namespace StoreManager.Domain.Common;

/// <summary>
/// ValueObjects cannot have an identity and are immutable. They are reliant on an entity. A ValueObject compared with another ValueObject with the same values are said to be equal
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var valueObject = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !Equals(left, right);
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }
}
