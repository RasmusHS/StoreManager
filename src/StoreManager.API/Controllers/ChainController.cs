using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Commands.Chain;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.Queries.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.API.Controllers;

[Route("api/chains")]
[ApiController]
public class ChainController : BaseController
{
    private IDispatcher _dispatcher;
    
    public ChainController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [Route("postChain")]
    public async Task<IActionResult> PostChain(CreateChainDto request)
    {
        bool containsStores = request.Stores != null && request.Stores.Any();
        CreateChainDto.Validator validator = new CreateChainDto.Validator(containsStores);
        var result = await validator.ValidateAsync(request);
        List<string> errors = new List<string>();

        if (!result.IsValid)
        {
            errors.AddRange(result.Errors.Select(e => e.ErrorMessage));
            return Error(string.Join(" | ", errors));
        }

        if (containsStores == true)
        {
            // Validate each store's value objects
            for (int i = 0; i < request.Stores!.Count; i++)
            {
                var store = request.Stores[i];
                var addressResult = Address.Create(store.Street, store.PostalCode, store.City);
                if (!addressResult.Success)
                {
                    errors.Add($"Store {i + 1} Address Error: {string.Join("; ", addressResult.Error.Code, addressResult.Error.Message, addressResult.Error.StatusCode)}");
                }
                var phoneResult = PhoneNumber.Create(store.CountryCode, store.PhoneNumber);
                if (!phoneResult.Success)
                {
                    errors.Add($"Store {i + 1} Phone Number Error: {string.Join("; ", phoneResult.Error.Code, phoneResult.Error.Message, phoneResult.Error.StatusCode)}");
                }
                var emailResult = Email.Create(store.Email);
                if (!emailResult.Success)
                {
                    errors.Add($"Store {i + 1} Email Error: {string.Join("; ", emailResult.Error.Code, emailResult.Error.Message, emailResult.Error.StatusCode)}");
                }
                var fullNameResult = FullName.Create(store.FirstName, store.LastName);
                if (!fullNameResult.Success)
                {
                    errors.Add($"Store {i + 1} Store Owner Name Error: {string.Join("; ", fullNameResult.Error.Code, fullNameResult.Error.Message, fullNameResult.Error.StatusCode)}");
                }
            }

            if (errors.Any())
            {
                return Error(string.Join(" | ", errors));
            }

            CreateChainCommand command = new CreateChainCommand(
                request.Name,
                request.Stores!.Select(s => new CreateStoreCommand(
                    ChainId.GetExisting(Guid.Empty).Value,
                    s.Number,
                    s.Name,
                    Address.Create(s.Street, s.PostalCode, s.City).Value,
                    PhoneNumber.Create(s.CountryCode, s.PhoneNumber).Value,
                    Email.Create(s.Email).Value,
                    FullName.Create(s.FirstName, s.LastName).Value
                    )).ToList());
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }
        else
        {
            CreateChainCommand command = new CreateChainCommand(
                request.Name,
                null);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }
    }

    [HttpGet]
    [Route("getChain/{chainId}")]
    public async Task<IActionResult> GetChainById(Guid chainId)
    {
        List<string> errors = new List<string>();

        var chainIdResult = ChainId.GetExisting(chainId);
        if (!chainIdResult.Success)
        {
            errors.Add(string.Join("; ", chainIdResult.Error.Code, chainIdResult.Error.Message, chainIdResult.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }

        var result = await _dispatcher.Dispatch(new GetChainQuery(chainIdResult.Value));
        if (result == null)
        {
            return Error($"Chain with ID {chainId} not found.");
        }

        if (result.Success)
        {
            return Ok(result.Value);
        }
        errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
        return Error(string.Join(" | ", errors));
    }

    [HttpGet]
    [Route("getChainAndStores/{chainId}")]
    public async Task<IActionResult> GetChainAndStores(Guid chainId)
    {
        List<string> errors = new List<string>();

        var chainIdResult = ChainId.GetExisting(chainId);
        if (!chainIdResult.Success)
        {
            errors.Add(string.Join("; ", chainIdResult.Error.Code, chainIdResult.Error.Message, chainIdResult.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }

        var result = await _dispatcher.Dispatch(new GetChainAndStoresQuery(chainIdResult.Value));
        if (result.Success)
        {
            return Ok(result.Value);
        }
        errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
        return Error(string.Join(" | ", errors));
    }

    [HttpGet]
    [Route("getAllChains")]
    public async Task<IActionResult> GetAllChains()
    {
        List<string> errors = new List<string>();

        var result = await _dispatcher.Dispatch(new GetAllChainsQuery());
        if (result.Success)
        {
            return Ok(result.Value);
        }
        errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
        return Error(string.Join(" | ", errors));
    }

    [HttpPut]
    [Route("putChain")]
    public async Task<IActionResult> PutChain(UpdateChainDto request)
    {
        UpdateChainDto.Validator validator = new UpdateChainDto.Validator();
        var result = await validator.ValidateAsync(request);
        List<string> errors = new List<string>();

        if (!result.IsValid)
        {
            errors.AddRange(result.Errors.Select(e => e.ErrorMessage));
            return Error(string.Join(" | ", errors));
        }

        var chainIdResult = ChainId.GetExisting(request.Id);
        if (!chainIdResult.Success)
        {
            errors.Add(string.Join("; ", chainIdResult.Error.Code, chainIdResult.Error.Message, chainIdResult.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }

        UpdateChainCommand command = new UpdateChainCommand(
                chainIdResult,
                request.Name,
                request.CreatedOn,
                request.ModifiedOn);
        var commandResult = await _dispatcher.Dispatch(command);
        if (commandResult.Success)
        {
            return Ok(commandResult.Value);
        }
        errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
        return Error(string.Join(" | ", errors));
    }

    [HttpDelete]
    [Route("deleteChain")]
    public async Task<IActionResult> DeleteChain(Guid chainId)
    {
        List<string> errors = new List<string>();

        // A chain can only be deleted if it has no associated stores
        var result = ChainId.GetExisting(chainId);
        if (!result.Success)
        {
            errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
            return Error(string.Join(" | ", errors));
        }

        DeleteChainCommand command = new DeleteChainCommand(result);
        var commandResult = await _dispatcher.Dispatch(command);
        if (commandResult.Success)
        {
            return Ok(commandResult);
        }
        errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
        return Error(string.Join(" | ", errors));
    }
}
