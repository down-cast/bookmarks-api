using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository;
using Downcast.Common.Errors;
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

    public Task<BookmarkDto> GetByArticleId(string articleId)
    {
        return _repository.GetByUserIdAndArticleId(_context.HttpContext.User.UserId(), articleId);
    }

    public Task<string> Create(BookmarkInputDto bookmark)
    {
        return _repository.Create(_context.HttpContext.User.UserId(), bookmark.ArticleId);
    }

    public Task<IEnumerable<BookmarkDto>> GetAll()
    {
        return _repository.GetAllByUserId(_context.HttpContext.User.UserId());
    }

    public Task Delete(string articleId)
    {
        return _repository.Delete(_context.HttpContext.User.UserId(), articleId);
    }

    public Task DeleteAllByUserId(string userId)
    {
        return _repository.GetAllByUserId(userId);
    }

    public async Task<BookmarkDto> GetById(string id)
    {
        BookmarkDto bookmark = await _repository.GetById(id).ConfigureAwait(false);
        string userId = _context.HttpContext.User.UserId();
        if (bookmark.UserId.Equals(userId, StringComparison.Ordinal))
        {
            return bookmark;
        }

        _logger.LogWarning("{BookmarkUserId} is different from the {UserId} in claims", bookmark.UserId, userId);
        throw new DcException(ErrorCodes.EntityNotFound, $"Could not find bookmark with id: {id}");
    }

    public async Task DeleteById(string id)
    {
        BookmarkDto bookmark = await _repository.GetById(id).ConfigureAwait(false);
        string userId = _context.HttpContext.User.UserId();
        if (!bookmark.UserId.Equals(userId, StringComparison.Ordinal))
        {
            _logger.LogWarning("{BookmarkUserId} is different from the {UserId} in claims", bookmark.UserId, userId);
            throw new DcException(ErrorCodes.EntityNotFound, $"Could not find bookmark with id: {id}");
        }
    }
}