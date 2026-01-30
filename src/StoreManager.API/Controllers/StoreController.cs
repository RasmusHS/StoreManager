using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Application.Queries.Store;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.API.Controllers;

[Route("api/stores")]
[ApiController]
public class StoreController : BaseController
{
    private IDispatcher _dispatcher;

    public StoreController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    [Route("postStore")]
    public async Task<IActionResult> PostStore(CreateStoreDto request)
    {
        CreateStoreDto.Validator validator = new CreateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);
        List<string> errors = new List<string>();

        if (!result.IsValid)
        {
            errors.AddRange(result.Errors.Select(e => "FluentValidation errors: \n" + " - Error code: " + e.ErrorCode + "\n - Error message: " + e.ErrorMessage + "\n"));
            return Error(errors);
        }

        // Validate value objects
        var addressResult = Address.Create(request.Street, request.PostalCode, request.City);
        var phoneResult = PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
        var emailResult = Email.Create(request.Email);
        var nameResult = FullName.Create(request.FirstName, request.LastName);

        if (!addressResult.Success)
            errors.Add(string.Join("; ", addressResult.Error.Code, addressResult.Error.Message, addressResult.Error.StatusCode));
        if (!phoneResult.Success)
            errors.Add(string.Join("; ", phoneResult.Error.Code, phoneResult.Error.Message, phoneResult.Error.StatusCode));
        if (!emailResult.Success)
            errors.Add(string.Join("; ", emailResult.Error.Code, emailResult.Error.Message, emailResult.Error.StatusCode));
        if (!nameResult.Success)
            errors.Add(string.Join("; ", nameResult.Error.Code, nameResult.Error.Message, nameResult.Error.StatusCode));

        if (errors.Any())
        {
            return Error(errors);
        }

        if (request.ChainId == null || request.ChainId == Guid.Empty)
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
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(errors);
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
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(errors);
        }
    }

    [HttpPost]
    [Route("postBulkStores")]
    public async Task<IActionResult> PostBulkStores(List<CreateStoreDto> requests)
    {
        List<CreateStoreDto.Validator> validators = requests.Select(_ => new CreateStoreDto.Validator()).ToList();
        List<CreateStoreCommand> commands = new List<CreateStoreCommand>();
        List<string> errors = new List<string>();
        
        // Check if all ChainIds are identical
        bool hasIdenticalChainIds = requests.Select(r => r.ChainId).Distinct().Count() <= 1;

        if (!hasIdenticalChainIds)
        {
            return Error("All stores in bulk creation must either share the same ChainId or none at all.");
        }

        // Get ChainId value
        Guid? commonChainId = requests.First().ChainId;

        for (int i = 0; i < requests.Count; i++)
        {
            var result = await validators[i].ValidateAsync(requests[i]);
            if (!result.IsValid)
            {
                errors.AddRange(result.Errors.Select(e => "FluentValidation errors: \n" + " - Error code: " + e.ErrorCode + "\n - Error message: " + e.ErrorMessage + "\n"));
                continue;
            }

            // Validate value objects
            var addressResult = Address.Create(requests[i].Street, requests[i].PostalCode, requests[i].City);
            var phoneResult = PhoneNumber.Create(requests[i].CountryCode, requests[i].PhoneNumber);
            var emailResult = Email.Create(requests[i].Email);
            var nameResult = FullName.Create(requests[i].FirstName, requests[i].LastName);

            if (!addressResult.Success)
                errors.Add(string.Join("; ", addressResult.Error.Code, addressResult.Error.Message, addressResult.Error.StatusCode));
            if (!phoneResult.Success)
                errors.Add(string.Join("; ", phoneResult.Error.Code, phoneResult.Error.Message, phoneResult.Error.StatusCode));
            if (!emailResult.Success)
                errors.Add(string.Join("; ", emailResult.Error.Code, emailResult.Error.Message, emailResult.Error.StatusCode));
            if (!nameResult.Success)
                errors.Add(string.Join("; ", nameResult.Error.Code, nameResult.Error.Message, nameResult.Error.StatusCode));

            if (commonChainId == null || commonChainId == Guid.Empty)
            {
                commands.Add(new CreateStoreCommand(
                    null,
                    requests[i].Number,
                    requests[i].Name,
                    addressResult.Value,
                    phoneResult.Value,
                    emailResult.Value,
                    nameResult.Value));
            }
            else
            {
                commands.Add(new CreateStoreCommand(
                    ChainId.GetExisting(commonChainId!.Value).Value,
                    requests[i].Number,
                    requests[i].Name,
                    addressResult.Value,
                    phoneResult.Value,
                    emailResult.Value,
                    nameResult.Value));
            }
        }

        if (errors.Any())
        {
            return Error(errors);
        }

        var bulkCommand = new BulkCreateStoresCommand(commands);
        var commandResults = await _dispatcher.Dispatch(bulkCommand);
        if (commandResults.Success)
        {
            return Ok(commandResults.Value);
        }
        errors.Add(string.Join("; ", commandResults.Error.Code, commandResults.Error.Message, commandResults.Error.StatusCode));
        return Error(errors);
    }

    [HttpGet]
    [Route("getStore/{storeId}")]
    public async Task<IActionResult> GetStore(Guid storeId)
    {
        List<string> errors = new List<string>();

        var storeIdResult = StoreId.GetExisting(storeId);
        if (!storeIdResult.Success)
        {
            errors.Add(string.Join("; ", storeIdResult.Error.Code, storeIdResult.Error.Message, storeIdResult.Error.StatusCode));
            return Error(errors);
        }

        var result = await _dispatcher.Dispatch(new GetStoreQuery(storeIdResult.Value));
        if (result.Success)
        {
            return Ok(result.Value);
        }
        errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
        return Error(errors);
    }

    [HttpGet]
    [Route("getStoresByChain/{chainId}")]
    public async Task<IActionResult> GetStoresByChainId(Guid chainId)
    {
        List<string> errors = new List<string>();

        var chainIdResult = ChainId.GetExisting(chainId);
        if (!chainIdResult.Success)
        {
            errors.Add(string.Join("; ", chainIdResult.Error.Code, chainIdResult.Error.Message, chainIdResult.Error.StatusCode));
            return Error(errors);
        }

        var result = await _dispatcher.Dispatch(new GetAllStoresByChainQuery(chainIdResult.Value));
        if (result.Success)
        {
            return Ok(result.Value);
        }
        errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
        return Error(errors);
    }

    [HttpPut]
    [Route("putStore")]
    public async Task<IActionResult> PutStore(UpdateStoreDto request)
    {
        UpdateStoreDto.Validator validator = new UpdateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);
        List<string> errors = new List<string>();

        if (!result.IsValid)
        {
            errors.AddRange(result.Errors.Select(e => "FluentValidation errors: \n" + " - Error code: " + e.ErrorCode + "\n - Error message: " + e.ErrorMessage + "\n"));
            return Error(errors);
        }

        // Validate value objects
        var storeIdResult = StoreId.GetExisting(request.Id);
        var addressResult = Address.Create(request.Street, request.PostalCode, request.City);
        var phoneResult = PhoneNumber.Create(request.CountryCode, request.PhoneNumber);
        var emailResult = Email.Create(request.Email);
        var nameResult = FullName.Create(request.FirstName, request.LastName);

        if (!storeIdResult.Success)
            errors.Add(string.Join("; ", storeIdResult.Error.Code, storeIdResult.Error.Message, storeIdResult.Error.StatusCode));
        if (!addressResult.Success)
            errors.Add(string.Join("; ", addressResult.Error.Code, addressResult.Error.Message, addressResult.Error.StatusCode));
        if (!phoneResult.Success)
            errors.Add(string.Join("; ", phoneResult.Error.Code, phoneResult.Error.Message, phoneResult.Error.StatusCode));
        if (!emailResult.Success)
            errors.Add(string.Join("; ", emailResult.Error.Code, emailResult.Error.Message, emailResult.Error.StatusCode));
        if (!nameResult.Success)
            errors.Add(string.Join("; ", nameResult.Error.Code, nameResult.Error.Message, nameResult.Error.StatusCode));

        if (request.ChainId != null && request.ChainId != Guid.Empty)
        {
            var chainIdResult = ChainId.GetExisting(request.ChainId!.Value);
            if (!chainIdResult.Success)
                errors.Add(string.Join("; ", chainIdResult.Error.Code, chainIdResult.Error.Message, chainIdResult.Error.StatusCode));
        }

        // Validate ChainId is not null before accessing .Value
        if (request.ChainId == null || request.ChainId == Guid.Empty)
        {
            UpdateStoreCommand command = new UpdateStoreCommand(
                storeIdResult.Value,
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
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(errors);
        }
        else
        {
            UpdateStoreCommand command = new UpdateStoreCommand(
                storeIdResult.Value,
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
            errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
            return Error(errors);
        }
    }

    [HttpDelete]
    [Route("deleteStore/{storeId}")]
    public async Task<IActionResult> DeleteStore(Guid storeId)
    {
        List<string> errors = new List<string>();

        var result = StoreId.GetExisting(storeId);
        if (!result.Success)
        {
            errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
            return Error(errors);
        }

        DeleteStoreCommand command = new DeleteStoreCommand(result);
        var commandResult = await _dispatcher.Dispatch(command);
        if (commandResult.Success)
        {
            return Ok(commandResult);
        }
        errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
        return Error(errors);
    }

    [HttpDelete]
    [Route("deleteAllStores/{chainId}")]
    public async Task<IActionResult> DeleteAllStores(Guid chainId)
    {
        List<string> errors = new List<string>();

        var result = ChainId.GetExisting(chainId);
        if (!result.Success) 
        {
            errors.Add(string.Join("; ", result.Error.Code, result.Error.Message, result.Error.StatusCode));
            return Error(errors);
        }

        DeleteAllStoresCommand command = new DeleteAllStoresCommand(result);
        var commandResult = await _dispatcher.Dispatch(command);
        if (commandResult.Success)
        {
            return Ok(commandResult);
        }
        errors.Add(string.Join("; ", commandResult.Error.Code, commandResult.Error.Message, commandResult.Error.StatusCode));
        return Error(errors);
    }
}
