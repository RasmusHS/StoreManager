namespace StoreManager.Domain.Common;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; }
    public DateTime CreatedOn { get; protected set; }
    public DateTime ModifiedOn { get; protected set; }

    protected Entity() { } // For ORM

    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// Entities have both referential equality as well as identifier equality
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
