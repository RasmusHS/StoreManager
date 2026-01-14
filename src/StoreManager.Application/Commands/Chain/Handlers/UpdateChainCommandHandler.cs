using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Commands.Chain.Handlers;

public class UpdateChainCommandHandler : ICommandHandler<UpdateChainCommand>
{
    private readonly IChainRepository _chainRepository;
    
    public UpdateChainCommandHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result> Handle(UpdateChainCommand command, CancellationToken cancellationToken = default)
    {
        var chainResult = await _chainRepository.GetByIdAsync(command.Id) ?? throw new KeyNotFoundException($"Chain with Id {command.Id} was not found.");

        chainResult.UpdateChainDetails(
            command.Name);

        await _chainRepository.UpdateAsync(chainResult, cancellationToken);

        return Result.Ok();
    }
}
