using System.Net;

using Downcast.Bookmarks.Client.Model;
using Downcast.Bookmarks.Tests.Utils;
using Downcast.Bookmarks.Tests.Utils.DataFakers;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Refit;

namespace Downcast.Bookmarks.Tests;

public class GetByArticleIdTests : BaseTestClass
{
    [Fact]
    public async Task GetByArticleId_Success()
    {
        string[] roles = { RoleNames.Admin, RoleNames.Member };
        TokenInfo tokenInfo = GenerateToken(roles);
        BookmarkInput bookmarkInput = new BookmarkInputFaker().Generate();

        HttpResponseMessage response = await Client.AddBookmark(bookmarkInput, tokenInfo.Token).ConfigureAwait(false);
        response.IsSuccessStatusCode.Should().BeTrue();

        ApiResponse<Bookmark> bookmark =
            await Client.GetBookmark(bookmarkInput.ArticleId, tokenInfo.Token).ConfigureAwait(false);

        bookmark.IsSuccessStatusCode.Should().BeTrue();
        bookmark.Content!.ArticleId.Should().Be(bookmarkInput.ArticleId);
    }

    [Fact]
    public async Task GetByArticleId_NotFound()
    {
        string[] roles = { RoleNames.Admin, RoleNames.Member };
        TokenInfo tokenInfo = GenerateToken(roles);
        ApiResponse<Bookmark> bookmark =
            await Client.GetBookmark(Faker.Random.Hexadecimal(10), tokenInfo.Token).ConfigureAwait(false);

        bookmark.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByArticleId_Invalid_Token_Returns_Unauthorized()
    {
        ApiResponse<Bookmark> bookmark = await Client.GetBookmark(
                Faker.Random.Hexadecimal(10),
                GenerateInvalidToken().Token)
            .ConfigureAwait(false);

        bookmark.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}