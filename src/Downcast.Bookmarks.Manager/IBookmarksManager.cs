using Downcast.Bookmarks.Model;

namespace Downcast.Bookmarks.Manager;

public interface IBookmarksManager
{
    Task<BookmarkDto> GetByArticleId(string userId, string articleId);
    Task<string> Create(string userId, BookmarkInputDto bookmark);
    IAsyncEnumerable<BookmarkDto> GetBookmarks(string userId, BookmarksFilter filter);
    Task Delete(string userId, string articleId);
    Task DeleteAll(string userId);
}