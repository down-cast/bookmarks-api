using System.Net;

using Downcast.Bookmarks.Client.Model;
using Downcast.Bookmarks.Tests.Utils;
using Downcast.Bookmarks.Tests.Utils.DataFakers;
using Downcast.SessionManager.SDK.Authentication.Extensions;

namespace Downcast.Bookmarks.Tests;

public class CreateBookmarkTest : BaseTestClass
{
    [Fact]
    public async Task CreateBookmark_Success()
    {
        string[] roles = { RoleNames.Admin, RoleNames.Member };
        TokenInfo tokenInfo = GenerateToken(roles);
        BookmarkInput? bookmarkInput = new BookmarkInputFaker().Generate();

        HttpResponseMessage response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBookmark_Success_Idempotent()
    {
        string[] roles = { RoleNames.Admin, RoleNames.Member };
        TokenInfo tokenInfo = GenerateToken(roles);
        BookmarkInput? bookmarkInput = new BookmarkInputFaker().Generate();

        HttpResponseMessage response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();
        response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();
    }


    [Fact]
    public async Task CreateBookmark_Fail_With_Invalid_Token()
    {
        TokenInfo tokenInfo = GenerateInvalidToken();
        BookmarkInput? bookmarkInput = new BookmarkInputFaker().Generate();
        HttpResponseMessage response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}