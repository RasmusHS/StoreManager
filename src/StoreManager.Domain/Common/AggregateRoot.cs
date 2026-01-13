namespace StoreManager.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot() { } // For ORM
    protected AggregateRoot(TId id) : base(id)
    {
    }
}
