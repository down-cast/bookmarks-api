using Downcast.Bookmarks.Manager;
using Downcast.Bookmarks.Model;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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


    [HttpGet]
    public Task<IEnumerable<BookmarkDto>> GetUserBookmarks()
    {
        return _manager.GetAll();
    }

    [HttpGet("{bookmarkId}")]
    public Task<BookmarkDto> GetById(string bookmarkId)
    {
        return _manager.GetById(bookmarkId);
    }

    [HttpDelete("{bookmarkId}")]
    public Task DeleteById(string bookmarkId)
    {
        return _manager.DeleteById(bookmarkId);
    }


    [HttpGet("article/{articleId}")]
    public Task<BookmarkDto> GetByArticleId(string articleId)
    {
        return _manager.GetByArticleId(articleId);
    }


    [HttpDelete("article/{articleId}")]
    public Task DeleteBookmark(string articleId)
    {
        return _manager.Delete(articleId);
    }


    [HttpPost]
    public async Task<ActionResult> CreateBookmark(BookmarkInputDto bookmark)
    {
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, User).ConfigureAwait(false);
        string _ = await _manager.Create(bookmark).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByArticleId), new { bookmark.ArticleId }, null);
    }


    [HttpDelete("user/{userId}")]
    [Authorize(Roles = RoleNames.Admin)]
    public Task DeleteBookmarks(string userId)
    {
        return _manager.DeleteAllByUserId(userId);
    }
}