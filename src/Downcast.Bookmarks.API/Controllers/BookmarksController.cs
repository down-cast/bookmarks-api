using Downcast.Bookmarks.Manager;
using Downcast.Bookmarks.Model;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Downcast.Bookmarks.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarksManager _manager;

    public BookmarksController(IBookmarksManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// Returns the user id associated with the session
    /// </summary>
    private string UserId => HttpContext.User.UserId();


    /// <summary>
    /// Returns user bookmarks as a stream of objects
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet]
    public IAsyncEnumerable<BookmarkDto> GetUserBookmarks([FromQuery] BookmarksFilter filter)
    {
        return _manager.GetBookmarks(UserId, filter);
    }

    /// <summary>
    /// Returns a bookmark by article id
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns></returns>
    [HttpGet("article/{articleId}")]
    public Task<BookmarkDto> GetByArticleId(string articleId)
    {
        return _manager.GetByArticleId(UserId, articleId);
    }


    /// <summary>
    /// Deletes bookmark by article id
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns></returns>
    [HttpDelete("article/{articleId}")]
    public Task DeleteBookmark(string articleId)
    {
        return _manager.Delete(UserId, articleId);
    }


    /// <summary>
    /// Adds a new bookmark for a given article Id
    /// </summary>
    /// <param name="bookmark"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CreateBookmark(BookmarkInputDto bookmark)
    {
        string id = await _manager.Create(UserId, bookmark).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByArticleId), new { articleId = id }, null);
    }


    /// <summary>
    /// Deletes all user bookmarks
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("user/{userId}")]
    [Authorize(Roles = RoleNames.Admin)]
    public Task DeleteBookmarks(string userId)
    {
        return _manager.DeleteAll(userId);
    }
}