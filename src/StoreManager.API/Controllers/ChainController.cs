using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Domain.Common;

namespace StoreManager.API.Controllers;

[Route("api/chain")]
[ApiController]
public class ChainController : BaseController
{
    private IDispatcher _dispatcher;
    
    public ChainController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [Route("createChain")]
    public async Task<IActionResult> CreateChain(CreateChainDto request)
    {
        CreateChainDto.Validator validator = new CreateChainDto.Validator();
        var result = await validator.ValidateAsync(request);

        //CreateChainCommand command = new Create Command(
        //ChainId.GetExisting(request.ChainId!.Value).Value,
        //);

        //var commandResult = await _dispatcher.Dispatch(command);
        //if (commandResult.Success)
        //{
        //    return Ok(commandResult);
        //}
        //return BadRequest(commandResult.Error.Code);

        return Ok();
    }

    [HttpGet]
    [Route("getChain/{chainId}")]
    public async Task<IActionResult> GetChain(Guid chainId)
    {
        // Implementation for getting a chain would go here

        //var result = await _dispatcher.Dispatch() ?? throw new KeyNotFoundException($" with ID {} not found.");
        //if (result.Success)
        //{
        //    return Ok(result.Value);
        //}
        //return BadRequest(result.Error.Code);

        return Ok();
    }

    [HttpGet]
    [Route("getChainAndStores/{chainId}")]
    public async Task<IActionResult> GetChainIncludeStores(Guid chainId)
    {
        // Implementation for getting a chain would go here

        //var result = await _dispatcher.Dispatch() ?? throw new KeyNotFoundException($" with ID {} not found.");
        //if (result.Success)
        //{
        //    return Ok(result.Value);
        //}
        //return BadRequest(result.Error.Code);

        return Ok();
    }

    [HttpPut]
    [Route("updateChain")]
    public async Task<IActionResult> UpdateChain(UpdateChainDto request)
    {
        // Implementation for updating a chain would go here
        UpdateChainDto.Validator validator = new UpdateChainDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }

    [HttpDelete]
    [Route("deleteChain")]
    public async Task<IActionResult> DeleteChain(DeleteChainDto request)
    {
        // Implementation for deleting a chain would go here
        // A chain can only be deleted if it has no associated stores
        DeleteChainDto.Validator validator = new DeleteChainDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }
}
