using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store.Handlers;

public class DeleteStoreCommandHandler : ICommandHandler<DeleteStoreCommand>
{
    private readonly IStoreRepository _storeRepository;
    
    public DeleteStoreCommandHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result> Handle(DeleteStoreCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_storeRepository.GetByIdAsync(command.Id).Result == null)
            {
                return Result.Fail(Errors.General.NotFound<StoreId>(command.Id));
            }
            await _storeRepository.DeleteAsync(command.Id, cancellationToken);

            return Result.Ok(); 
        }
        catch (Exception ex)
        {
            return Result.Fail(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
