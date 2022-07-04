using Downcast.Bookmarks.Model;

namespace Downcast.Bookmarks.Manager;

public interface IBookmarksManager
{
    Task<BookmarkDto> GetByArticleId(string articleId);
    Task<string> Create(BookmarkInputDto bookmark);
    Task<IEnumerable<BookmarkDto>> GetAll();
    Task Delete(string articleId);
    Task DeleteAllByUserId(string userId);
}