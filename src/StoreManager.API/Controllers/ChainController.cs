using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Commands.Chain;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.Queries.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;

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
        bool containsStores = request.Stores != null && request.Stores.Any();
        CreateChainDto.Validator validator = new CreateChainDto.Validator(containsStores);
        var result = await validator.ValidateAsync(request);

        if (result.IsValid)
        {
            if (containsStores == true)
            {
                CreateChainCommand command = new CreateChainCommand(
                    request.Name,
                    request.Stores!.Select(s => new CreateStoreCommand(
                        ChainId.GetExisting(s.ChainId!.Value).Value,
                        s.Number,
                        s.Name,
                        Address.Create(s.Street, s.PostalCode, s.City),
                        PhoneNumber.Create(s.CountryCode, s.PhoneNumber),
                        Email.Create(s.Email),
                        FullName.Create(s.FirstName, s.LastName)
                        )).ToList());
                var commandResult = await _dispatcher.Dispatch(command);
                if (commandResult.Success)
                {
                    return Ok(commandResult);
                }
            }
            else
            {
                CreateChainCommand command = new CreateChainCommand(
                    request.Name,
                    null);
                var commandResult = await _dispatcher.Dispatch(command);
                if (commandResult.Success)
                {
                    return Ok(commandResult);
                }
            }
        }
        return BadRequest(result.Errors);
    }

    [HttpGet]
    [Route("getChain/{chainId}")]
    public async Task<IActionResult> GetChainById(Guid chainId)
    {
        var result = await _dispatcher.Dispatch(new GetChainQuery(ChainId.GetExisting(chainId).Value)) ?? throw new KeyNotFoundException($"Chain with ID {chainId} not found.");
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error.Code);
    }

    [HttpGet]
    [Route("getChainAndStores/{chainId}")]
    public async Task<IActionResult> GetChainIncludeStores(Guid chainId)
    {
        var result = await _dispatcher.Dispatch(new GetChainAndStoresQuery(ChainId.GetExisting(chainId).Value)) ?? throw new KeyNotFoundException($"Chain with ID {chainId} not found.");
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error.Code);
    }

    [HttpPut]
    [Route("updateChain")]
    public async Task<IActionResult> UpdateChain(UpdateChainDto request)
    {
        UpdateChainDto.Validator validator = new UpdateChainDto.Validator();
        var result = await validator.ValidateAsync(request);
        if (result.IsValid)
        {
            UpdateChainCommand command = new UpdateChainCommand(
                ChainId.GetExisting(request.Id),
                request.Name,
                request.CreatedOn,
                request.ModifiedOn);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult);
            }
            return BadRequest(commandResult.Error.Code);
        }
        return BadRequest(result.Errors);
    }

    [HttpDelete]
    [Route("deleteChain")]
    public async Task<IActionResult> DeleteChain(DeleteChainDto request)
    {
        // A chain can only be deleted if it has no associated stores
        DeleteChainDto.Validator validator = new DeleteChainDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }
}
