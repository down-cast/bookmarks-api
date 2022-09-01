using System.Net;

using Downcast.Bookmarks.Client.Model;
using Downcast.Bookmarks.Tests.Utils;
using Downcast.Bookmarks.Tests.Utils.DataFakers;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Refit;

namespace Downcast.Bookmarks.Tests;

public class DeleteUserBookmarksTests : BaseTestClass
{
    [Fact]
    public async Task DeleteUserBookmarks_Success()
    {
        // Create many bookmarks
        TokenInfo tokenInfo = GenerateToken(new[] { RoleNames.Admin, RoleNames.Member });
        List<BookmarkInput> bookmarks = new BookmarkInputFaker().Generate(15);
        await AddBookmarks(bookmarks, tokenInfo).ConfigureAwait(false);

        // Delete all bookmarks
        await Client.DeleteUserBookmarks(tokenInfo.UserId, tokenInfo.Token).ConfigureAwait(false);

        // Ensure each one of them is deleted
        foreach (BookmarkInput bookmarkInput in bookmarks)
        {
            ApiResponse<Bookmark> response =
                await Client.GetBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task DeleteUserBookmarks_One_User_Does_Not_Affect_Another()
    {
        // Create many bookmarks
        TokenInfo tokenInfo = GenerateToken(new[] { RoleNames.Admin, RoleNames.Member });
        List<BookmarkInput> bookmarks = new BookmarkInputFaker().Generate(15);
        await AddBookmarks(bookmarks, tokenInfo).ConfigureAwait(false);

        // Create many bookmarks for another user
        TokenInfo token2 = GenerateToken(new[] { RoleNames.Admin, RoleNames.Member });
        await AddBookmarks(bookmarks, token2).ConfigureAwait(false);

        // Delete all bookmarks from the first user
        await Client.DeleteUserBookmarks(tokenInfo.UserId, tokenInfo.Token).ConfigureAwait(false);

        // Ensure each one of them is deleted from first user
        foreach (BookmarkInput bookmarkInput in bookmarks)
        {
            ApiResponse<Bookmark> response =
                await Client.GetBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Ensure each one of them is present from second user user
        foreach (BookmarkInput bookmarkInput in bookmarks)
        {
            ApiResponse<Bookmark> response =
                await Client.GetBookmark(bookmarkInput.ArticleId, token2.Token).ConfigureAwait(false);
            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }

    private Task AddBookmarks(IEnumerable<BookmarkInput> bookmarks, TokenInfo tokenInfo)
    {
        IEnumerable<Task<HttpResponseMessage>> addBookmarksTasks =
            bookmarks.Select(bookmark => Client.AddBookmark(bookmark, tokenInfo.Token));

        return Task.WhenAll(addBookmarksTasks);
    }
}