using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Application.Queries.Store;
using StoreManager.Application.Queries.Store.Handlers;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.API.Controllers;

[Route("api/store")]
[ApiController]
public class StoreController : BaseController
{
    private IDispatcher _dispatcher;

    public StoreController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [Route("createStore")]
    public async Task<IActionResult> CreateStore(CreateStoreDto request)
    {
        CreateStoreDto.Validator validator = new CreateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);

        CreateStoreCommand command = new CreateStoreCommand(
            ChainId.GetExisting(request.ChainId!.Value).Value,
            request.Number,
            request.Name,
            Address.Create(request.Street, request.PostalCode, request.City),
            PhoneNumber.Create(request.CountryCode, request.PhoneNumber),
            Email.Create(request.Email),
            FullName.Create(request.FirstName, request.LastName));
        var commandResult = await _dispatcher.Dispatch(command);
        if (commandResult.Success)
        {
            return Ok(commandResult);
        }
        return BadRequest(commandResult.Error.Code);
    }

    [HttpGet]
    [Route("getStore/{storeId}")]
    public async Task<IActionResult> GetStore(Guid storeId)
    {
        var result = await _dispatcher.Dispatch(new GetStoreQuery(StoreId.GetExisting(storeId).Value)) ?? throw new KeyNotFoundException($"Store with ID {storeId} not found.");
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error.Code);
    }

    [HttpGet]
    [Route("getStoresByChain/{chainId}")]
    public async Task<IActionResult> GetStoresByChainId(Guid chainId)
    {
        var result = await _dispatcher.Dispatch(new GetAllStoresByChainQuery(ChainId.GetExisting(chainId).Value)) ?? throw new KeyNotFoundException($"Stores belonging to chain with ID {chainId} not found.");
        if (result.Success)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Error.Code);
    }

    [HttpPut]
    [Route("updateStore")]
    public async Task<IActionResult> UpdateStore(UpdateStoreDto request)
    {
        UpdateStoreDto.Validator validator = new UpdateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);
        if (result.IsValid)
        {
            UpdateStoreCommand command = new UpdateStoreCommand(
                StoreId.GetExisting(request.Id).Value,
                ChainId.GetExisting(request.ChainId).Value,
                request.Number,
                request.Name,
                Address.Create(request.Street, request.PostalCode, request.City),
                PhoneNumber.Create(request.CountryCode, request.PhoneNumber),
                Email.Create(request.Email),
                FullName.Create(request.FirstName, request.LastName),
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
    [Route("deleteStore")]
    public async Task<IActionResult> DeleteStore(DeleteStoreDto request)
    {
        // Uses Id from DeleteStoreDto
        DeleteStoreDto.Validator validator = new DeleteStoreDto.Validator();
        var result = await validator.ValidateAsync(request);
        if (result.IsValid) 
        {
            DeleteStoreCommand command = new DeleteStoreCommand(StoreId.GetExisting(request.Id).Value);
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
    [Route("deleteAllStores")]
    public async Task<IActionResult> DeleteAllStores(DeleteStoreDto request)
    {
        // Uses ChainId from DeleteStoreDto
        DeleteStoreDto.Validator validator = new DeleteStoreDto.Validator();
        var result = await validator.ValidateAsync(request);
        if (result.IsValid)
        {
            DeleteStoreCommand command = new DeleteStoreCommand(ChainId.GetExisting(request.ChainId).Value);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult);
            }
            return BadRequest(commandResult.Error.Code);
        };
        return BadRequest(result.Errors);
    }
}
