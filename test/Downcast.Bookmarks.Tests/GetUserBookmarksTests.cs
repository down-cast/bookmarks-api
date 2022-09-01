using System.Net;

using Downcast.Bookmarks.Client.Model;
using Downcast.Bookmarks.Tests.Utils;
using Downcast.Bookmarks.Tests.Utils.DataFakers;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Refit;

namespace Downcast.Bookmarks.Tests;

public class GetUserBookmarksTests : BaseTestClass
{
    [Fact]
    public async Task GetUserBookmarks_Success()
    {
        TokenInfo tokenInfo = GenerateToken(new[] { RoleNames.Admin, RoleNames.Member });
        List<BookmarkInput> bookmarks = new BookmarkInputFaker().Generate(15);
        IEnumerable<Task<HttpResponseMessage>> addBookmarksTasks =
            bookmarks.Select(bookmark => Client.AddBookmark(bookmark, tokenInfo.Token));

        await Task.WhenAll(addBookmarksTasks).ConfigureAwait(false);

        ApiResponse<IEnumerable<Bookmark>> response = await Client
            .GetBookmarks(new BookmarksFilter { Top = 5, Skip = 5 }, tokenInfo.Token)
            .ConfigureAwait(false);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().HaveCount(5);
    }
}