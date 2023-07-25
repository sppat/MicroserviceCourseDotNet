using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Dtos;
using Post.Query.Api.Dtos;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to retrieve all posts";

            return ErrorResponse(ex, safeErrorMessage);

        }
    }

    [HttpGet("byId/{postId:guid}")]
    public async Task<ActionResult> GetByPostIdAsync(Guid postId)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostByIdQuery
            {
                Id = postId
            });

            if (posts is null || !posts.Any())
            {
                return NoContent();
            }

            return Ok(new PostLookupResponse
            {
                Posts = posts,
                Message = "Successfully returned post"
            });
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to find a post by Id";

            return ErrorResponse(ex, safeErrorMessage);

        }
    }
    
    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetByPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery
            {
                Author = author
            });

            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to find posts by author";

            return ErrorResponse(ex, safeErrorMessage);
        }
    }
    
    [HttpGet("withComments")]
    public async Task<ActionResult> GetByPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to find posts with comments";
            
            return ErrorResponse(ex, safeErrorMessage);
        }
    }
    
    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetByPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery
            {
                NumberOfLikes = numberOfLikes
            });

            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string safeErrorMessage = "Error while processing request to find posts with likes";
            
            return ErrorResponse(ex, safeErrorMessage);
        }
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
        _logger.LogError(ex, safeErrorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        });
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        if (posts is null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;

        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}"
        });
    }
}