using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Dtos;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestoreReadDbController : ControllerBase
{
    private readonly ILogger<RestoreReadDbController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreReadDbController(ICommandDispatcher commandDispatcher, ILogger<RestoreReadDbController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult> RestoreReadDbAsyncAsync()
    {
        var id = Guid.NewGuid();
        try
        {
            await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

            return StatusCode(StatusCodes.Status201Created, new BaseResponse()
            {
                Message = "Read database restore request completed successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request");

            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to restore read database";
            _logger.Log(LogLevel.Error, ex, safeErrorMessage);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
            {
                Message = safeErrorMessage
            });
        }
    }
}