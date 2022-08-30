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

    [HttpGet]
    public Task<IEnumerable<BookmarkDto>> GetUserBookmarks()
    {
        return _manager.GetAll(UserId);
    }

    [HttpGet("{bookmarkId}")]
    public Task<BookmarkDto> GetById(string bookmarkId)
    {
        return _manager.GetById(UserId, bookmarkId);
    }

    [HttpDelete("{bookmarkId}")]
    public Task DeleteById(string bookmarkId)
    {
        return _manager.DeleteById(UserId, bookmarkId);
    }


    [HttpGet("article/{articleId}")]
    public Task<BookmarkDto> GetByArticleId(string articleId)
    {
        return _manager.GetByArticleId(UserId, articleId);
    }


    [HttpDelete("article/{articleId}")]
    public Task DeleteBookmark(string articleId)
    {
        return _manager.Delete(UserId, articleId);
    }


    [HttpPost]
    public async Task<ActionResult> CreateBookmark(BookmarkInputDto bookmark)
    {
        string _ = await _manager.Create(UserId, bookmark).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByArticleId), new { bookmark.ArticleId }, null);
    }


    [HttpDelete("user/{userId}")]
    [Authorize(Roles = RoleNames.Admin)]
    public Task DeleteBookmarks(string userId)
    {
        return _manager.DeleteAllByUserId(userId);
    }
}