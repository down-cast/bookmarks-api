using Downcast.Bookmarks.Model;
using Downcast.Bookmarks.Repository;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.AspNetCore.Http;

namespace Downcast.Bookmarks.Manager;

public class BookmarksManager : IBookmarksManager
{
    private readonly IHttpContextAccessor _context;
    private readonly IBookmarksRepository _repository;

    public BookmarksManager(IHttpContextAccessor context, IBookmarksRepository repository)
    {
        _context    = context;
        _repository = repository;
    }

    public Task<BookmarkDto> GetByArticleId(string articleId)
    {
        return _repository.GetByUserIdAndArticleId(_context.HttpContext.User.GetRequiredUserId(), articleId);
    }

    public Task<string> Create(BookmarkInputDto bookmark)
    {
        return _repository.Create(_context.HttpContext.User.GetRequiredUserId(), bookmark.ArticleId);
    }

    public Task<IEnumerable<BookmarkDto>> GetAll()
    {
        return _repository.GetAllByUserId(_context.HttpContext.User.GetRequiredUserId());
    }

    public Task Delete(string articleId)
    {
        return _repository.Delete(_context.HttpContext.User.GetRequiredUserId(), articleId);
    }

    public Task DeleteAllByUserId(string userId)
    {
        return _repository.GetAllByUserId(userId);
    }
}