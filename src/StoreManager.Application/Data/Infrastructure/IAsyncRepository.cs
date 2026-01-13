namespace StoreManager.Application.Data.Infrastructure;

public interface IAsyncRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(object id);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(object id, CancellationToken cancellationToken = default);
    void Save(CancellationToken cancellationToken = default);
}
