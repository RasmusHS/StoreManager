using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;

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
        // Implementation for creating a store would go here
        CreateStoreDto.Validator validator = new CreateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetStore()
    {
        // Implementation for getting stores would go here

        //var result = await _dispatcher.Dispatch() ?? throw new KeyNotFoundException($" with ID {} not found.");
        //if (result.Success)
        //{
        //    return Ok(result.Value);
        //}
        //return BadRequest(result.Error.Code);

        return Ok();
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetStoresByChainId()
    {
        // Implementation for getting stores would go here

        //var result = await _dispatcher.Dispatch() ?? throw new KeyNotFoundException($" with ID {} not found.");
        //if (result.Success)
        //{
        //    return Ok(result.Value);
        //}
        //return BadRequest(result.Error.Code);

        return Ok();
    }

    [HttpPut]
    [Route("")]
    public async Task<IActionResult> UpdateStore(UpdateStoreDto request)
    {
        // Implementation for updating a store would go here
        UpdateStoreDto.Validator validator = new UpdateStoreDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }

    [HttpDelete]
    [Route("")]
    public async Task<IActionResult> DeleteStore(DeleteStoreDto request)
    {
        // Implementation for deleting a store would go here
        // Uses Id from DeleteStoreDto
        DeleteStoreDto.Validator validator = new DeleteStoreDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }

    [HttpDelete]
    [Route("")]
    public async Task<IActionResult> DeleteAllStores(DeleteStoreDto request)
    {
        // Implementation for deleting all stores would go here
        // Uses ChainId from DeleteStoreDto
        DeleteStoreDto.Validator validator = new DeleteStoreDto.Validator();
        var result = await validator.ValidateAsync(request);

        return Ok();
    }
}
