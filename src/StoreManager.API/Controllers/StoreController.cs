using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Application.Queries.Store;
using StoreManager.Application.Queries.Store.Handlers;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
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

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        // Validate value objects
        var addressResult = Address.Create(request.Street, request.PostalCode, request.City);
        var phoneResult = PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
        var emailResult = Email.Create(request.Email);
        var nameResult = FullName.Create(request.FirstName, request.LastName);

        if (!addressResult.Success)
            return BadRequest(addressResult.Error.Code);
        if (!phoneResult.Success)
            return BadRequest(phoneResult.Error.Code);
        if (!emailResult.Success)
            return BadRequest(emailResult.Error.Code);
        if (!nameResult.Success)
            return BadRequest(nameResult.Error.Code);

        if (request.ChainId == null)
        {
            CreateStoreCommand command = new CreateStoreCommand(
                null,
                request.Number,
                request.Name,
                addressResult.Value,
                phoneResult.Value,
                emailResult.Value,
                nameResult.Value);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            return BadRequest(commandResult.Error.Code);
        }
        else
        {
            CreateStoreCommand command = new CreateStoreCommand(
                ChainId.GetExisting(request.ChainId!.Value).Value,
                request.Number,
                request.Name,
                addressResult.Value,
                phoneResult.Value,
                emailResult.Value,
                nameResult.Value);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            return BadRequest(commandResult.Error.Code);
        }
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

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }

        // Validate value objects
        var addressResult = Address.Create(request.Street, request.PostalCode, request.City);
        var phoneResult = PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
        var emailResult = Email.Create(request.Email);
        var nameResult = FullName.Create(request.FirstName, request.LastName);

        if (!addressResult.Success)
            return BadRequest(addressResult.Error.Code);
        if (!phoneResult.Success)
            return BadRequest(phoneResult.Error.Code);
        if (!emailResult.Success)
            return BadRequest(emailResult.Error.Code);
        if (!nameResult.Success)
            return BadRequest(nameResult.Error.Code);

        // Validate ChainId is not null before accessing .Value
        if (request.ChainId == null)
        {
            UpdateStoreCommand command = new UpdateStoreCommand(
                StoreId.GetExisting(request.Id).Value,
                null,
                request.Number,
                request.Name,
                addressResult.Value,
                phoneResult.Value,
                emailResult.Value,
                nameResult.Value,
                request.CreatedOn,
                request.ModifiedOn);
            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            return BadRequest(commandResult.Error.Code);
        }
        else
        {
            UpdateStoreCommand command = new UpdateStoreCommand(
                StoreId.GetExisting(request.Id).Value,
                ChainId.GetExisting(request.ChainId.Value).Value,
                request.Number,
                request.Name,
                addressResult.Value,
                phoneResult.Value,
                emailResult.Value,
                nameResult.Value,
                request.CreatedOn,
                request.ModifiedOn);

            var commandResult = await _dispatcher.Dispatch(command);
            if (commandResult.Success)
            {
                return Ok(commandResult.Value);
            }
            return BadRequest(commandResult.Error.Code);
        }
    }

    [HttpDelete]
    [Route("deleteStore")]
    public async Task<IActionResult> DeleteStore(DeleteStoreDto request)
    {
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
    public async Task<IActionResult> DeleteAllStores(DeleteAllStoresDto request)
    {
        DeleteAllStoresDto.Validator validator = new DeleteAllStoresDto.Validator();
        var result = await validator.ValidateAsync(request);
        if (result.IsValid)
        {
            DeleteAllStoresCommand command = new DeleteAllStoresCommand(ChainId.GetExisting(request.ChainId).Value);
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
