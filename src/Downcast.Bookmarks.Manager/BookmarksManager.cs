using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Downcast.Bookmarks.Manager;

public class BookmarksManager : IBookmarksManager
{
    private readonly IHttpContextAccessor _context;
    private readonly IBookmarksRepository _repository;
    private readonly ILogger<BookmarksManager> _logger;

    public BookmarksManager(
        IHttpContextAccessor context,
        IBookmarksRepository repository,
        ILogger<BookmarksManager> logger)
    {
        _context = context;
        _repository = repository;
        _logger = logger;
    }

    public Task<BookmarkDto> GetByArticleId(string userId, string articleId)
    {
        return _repository.GetByUserIdAndArticleId(userId, articleId);
    }

    public Task<string> Create(string userId, BookmarkInputDto bookmark)
    {
        return _repository.Create(userId, bookmark.ArticleId);
    }

    public IAsyncEnumerable<BookmarkDto> GetBookmarks(string userId, BookmarksFilter filter)
    {
        return _repository.GetBookmarksByUserId(userId, filter);
    }

    public Task Delete(string userId, string articleId)
    {
        return _repository.Delete(userId, articleId);
    }

    public Task DeleteAllByUserId(string userId)
    {
        return _repository.DeleteAllByUserId(userId);
    }

    public Task<BookmarkDto> GetById(string userId, string bookmarkId)
    {
        return _repository.GetById(userId, bookmarkId);
    }

    public Task DeleteById(string userId, string bookmarkId)
    {
        return _repository.DeleteById(_context.HttpContext.User.UserId(), bookmarkId);
    }
}