using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Commands.Chain.Handlers;

public class UpdateChainCommandHandler : ICommandHandler<UpdateChainCommand, ChainResponseDto>
{
    private readonly IChainRepository _chainRepository;
    
    public UpdateChainCommandHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result<ChainResponseDto>> Handle(UpdateChainCommand command, CancellationToken cancellationToken = default)
    {
        var chainResult = await _chainRepository.GetByIdAsync(command.Id) ?? throw new KeyNotFoundException($"Chain with Id {command.Id} was not found.");

        chainResult.UpdateChainDetails(command.Name);

        // Check if UpdateChainCommand includes new stores to be added
        //if (command.StoresToAdd != null && command.StoresToAdd.Any())
        //{
        //    command.StoresToAdd.RemoveAll(s => s.ChainId == chainResult.Id);
        //    chainResult.ClearStoresFromChain();
        //    chainResult.AddRangeStoresToChain(command.StoresToAdd);
        //    await _chainRepository.AddStoresToChainAsync(chainResult, command.StoresToAdd);
        //}

        await _chainRepository.UpdateAsync(chainResult, cancellationToken);

        var responseDto = new ChainResponseDto
        {
            Id = chainResult.Id.Value,
            Name = chainResult.Name,
            CreatedOn = chainResult.CreatedOn,
            ModifiedOn = chainResult.ModifiedOn,
        };

        return Result.Ok<ChainResponseDto>(responseDto);
    }
}
