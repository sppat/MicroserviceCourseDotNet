using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Dtos;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditCommentController : ControllerBase
{
    private readonly ILogger<EditCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public EditCommentController(ICommandDispatcher commandDispatcher, ILogger<EditCommentController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> EditCommentAsync(Guid id, EditCommentCommand command)
    {
        try
        {
            command.Id = id;

            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Edit comment request completed successfully"
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
        catch (AggregateNotFoundException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed an incorrect post ID targeting the aggregate");

            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to edit a comment of a post";
            _logger.Log(LogLevel.Error, ex, safeErrorMessage);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = safeErrorMessage
            });
        }
    }
}