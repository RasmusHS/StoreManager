namespace StoreManager.Domain.Common;

public abstract class JoinEntity<TId1, TId2> : IEquatable<JoinEntity<TId1, TId2>>
    where TId1 : notnull
    where TId2 : notnull
{
    public TId1 Id1 { get; protected set; }
    public TId2 Id2 { get; protected set; }
    public DateTime CreatedOn { get; protected set; }
    public DateTime ModifiedOn { get; protected set; }

    protected JoinEntity() { } // For ORM
    protected JoinEntity(TId1 id1, TId2 id2)
    {
        Id1 = id1;
        Id2 = id2;
    }

    public override bool Equals(object? obj)
    {
        return obj is JoinEntity<TId1, TId2> entity &&
               Id1.Equals(entity.Id1) &&
               Id2.Equals(entity.Id2);
    }

    public bool Equals(JoinEntity<TId1, TId2>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(JoinEntity<TId1, TId2> left, JoinEntity<TId1, TId2> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(JoinEntity<TId1, TId2> left, JoinEntity<TId1, TId2> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id1, Id2);
    }
}
