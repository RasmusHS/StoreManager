using Microsoft.AspNetCore.Mvc;
using StoreManager.API.Utilities;
using StoreManager.Domain.Common;

namespace StoreManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected new ActionResult Ok()
    {
        return base.Ok(Envelope.Ok());
    }

    protected ActionResult Ok<T>(T result)
    {
        return base.Ok(Envelope.Ok(result));
    }

    protected ActionResult Error(List<string> errorMessages)
    {
        string errors = string.Join(" | ", errorMessages);
        return BadRequest(Envelope.Error(errors));
    }

    protected ActionResult Error(string errorMessage)
    {
        return BadRequest(Envelope.Error(errorMessage));
    }

    protected IActionResult FromResult(Result result)
    {
        if (result.Failure)
            return StatusCodeFromResult(result);
        return base.Ok(Envelope.Ok());
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.Failure)
            return StatusCodeFromResult(result);

        return base.Ok(Envelope.Ok(result.Value));
    }

    private IActionResult StatusCodeFromResult(Result result)
        => StatusCode(result.Error.StatusCode, Envelope.Error(result.Error.Code));
}
