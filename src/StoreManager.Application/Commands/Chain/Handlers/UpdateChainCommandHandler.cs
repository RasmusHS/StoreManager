using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Domain.Chain.ValueObjects;
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
        var chainResult = await _chainRepository.GetByIdAsync(command.Id);
        if (chainResult == null)
        {
            return Result.Fail<ChainResponseDto>(Errors.General.NotFound<ChainId>(command.Id));
        }

        chainResult.UpdateChainDetails(command.Name);

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
