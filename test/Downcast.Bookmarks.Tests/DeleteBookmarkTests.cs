using System.Net;

using Downcast.Bookmarks.Client.Model;
using Downcast.Bookmarks.Tests.Utils;
using Downcast.Bookmarks.Tests.Utils.DataFakers;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Refit;

namespace Downcast.Bookmarks.Tests;

public class DeleteBookmarkTests : BaseTestClass
{
    [Fact]
    public async Task DeleteBookmark_Success()
    {
        string[] roles = { RoleNames.Admin, RoleNames.Member };
        TokenInfo tokenInfo = GenerateToken(roles);
        BookmarkInput? bookmarkInput = new BookmarkInputFaker().Generate();

        // create bookmark
        HttpResponseMessage response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();

        // ensure it exists
        ApiResponse<Bookmark> bookmarkResponse =
            await Client.GetBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);
        bookmarkResponse.IsSuccessStatusCode.Should().BeTrue();
        bookmarkResponse.Content!.ArticleId.Should().Be(bookmarkInput.ArticleId);

        // delete bookmark
        response = await Client.DeleteBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();

        // ensure it no longer exists
        bookmarkResponse =
            await Client.GetBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);
        bookmarkResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBookmark_Non_Existent_Bookmark_Returns_OK()
    {
        TokenInfo tokenInfo = GenerateToken(new[] { RoleNames.Admin, RoleNames.Member });

        // delete bookmark
        HttpResponseMessage response = await Client.DeleteBookmark(Faker.Random.Guid().ToString(), tokenInfo.Token)
            .ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteBookmark_Invalid_Token_Returns_Unauthorized()
    {
        TokenInfo tokenInfo = GenerateInvalidToken();

        // delete bookmark
        HttpResponseMessage response = await Client.DeleteBookmark(Faker.Random.Guid().ToString(), tokenInfo.Token)
            .ConfigureAwait(false);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}