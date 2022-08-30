using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository;

using Microsoft.Extensions.Logging;

namespace Downcast.Bookmarks.Manager;

public class BookmarksManager : IBookmarksManager
{
    private readonly IBookmarksRepository _repository;
    private readonly ILogger<BookmarksManager> _logger;

    public BookmarksManager(
        IBookmarksRepository repository,
        ILogger<BookmarksManager> logger)
    {
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
        return _repository.GetBookmarks(userId, filter);
    }

    public Task Delete(string userId, string articleId)
    {
        return _repository.Delete(userId, articleId);
    }

    public Task DeleteAll(string userId)
    {
        return _repository.DeleteAll(userId);
    }
}