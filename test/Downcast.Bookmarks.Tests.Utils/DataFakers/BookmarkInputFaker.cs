using Bogus;

using Downcast.Bookmarks.Client.Model;

using Microsoft.IdentityModel.JsonWebTokens;

namespace Downcast.Bookmarks.Tests.Utils.DataFakers;

public sealed class BookmarkInputFaker : Faker<BookmarkInput>
{
    private readonly JsonWebTokenHandler _handler = new();

    public BookmarkInputFaker()
    {
        RuleFor(b => b.ArticleId, faker => faker.Random.Guid().ToString());
    }
}