using Downcast.Bookmarks.Model;

namespace Downcast.Bookmarks.Repository;

public interface IBookmarksRepository
{
    Task<string> Create(string userId, string articleId);
    IAsyncEnumerable<BookmarkDto> GetBookmarks(string userId, BookmarksFilter filter);
    Task Delete(string userId, string articleId);
    Task DeleteAll(string userId);
    Task<BookmarkDto> GetByUserIdAndArticleId(string userId, string articleId);
}