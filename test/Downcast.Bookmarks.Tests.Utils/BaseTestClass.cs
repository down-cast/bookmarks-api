using System.Text;

using Bogus;

using Downcast.Bookmarks.Client;
using Downcast.SessionManager.SDK.Authentication.Extensions;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Refit;

namespace Downcast.Bookmarks.Tests.Utils;

public class BaseTestClass
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly SecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is a test key"));
    protected Faker Faker { get; } = new();
    protected IBookmarksClient Client { get; }

    public BaseTestClass()
    {
        HttpClient httpClient = new BookmarksServerInstance().CreateClient();
        Client = RestService.For<IBookmarksClient>(httpClient);
    }

    protected TokenInfo GenerateToken(IEnumerable<string> roles)
    {
        string email = Faker.Internet.Email();
        var userId = Faker.Random.Guid().ToString();
        string name = Faker.Name.FullName();

        string token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires = DateTime.MaxValue,
            Claims = new Dictionary<string, object>
            {
                { ClaimNames.Role, roles },
                { ClaimNames.Email, email },
                { ClaimNames.DisplayName, name },
                { ClaimNames.UserId, userId }
            },
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature),
            Issuer = "test",
            IssuedAt = DateTime.UtcNow,
            Audience = "test"
        });


        return new TokenInfo
        {
            Token = token,
            UserId = userId,
            Roles = roles,
            Email = email
        };
    }

    protected TokenInfo GenerateInvalidToken()
    {
        TokenInfo tokenInfo = GenerateToken(new[] { RoleNames.Admin, RoleNames.Author });
        tokenInfo.Token = "aasd" + tokenInfo.Token;
        return tokenInfo;
    }
}