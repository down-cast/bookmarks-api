using System.Net;

using Downcast.Bookmarks.API.Controllers;
using Downcast.Bookmarks.Repository.Options;
using Downcast.SessionManager.SDK.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

using Moq;

using Refit;

namespace Downcast.Bookmarks.Tests.Utils;

public class BookmarksServerInstance : WebApplicationFactory<BookmarksController>
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly Mock<ISessionManagerClient> _sessionManagerMock = new(MockBehavior.Strict);

    public BookmarksServerInstance()
    {
        _sessionManagerMock.Setup(client => client.ValidateSessionToken(It.IsAny<string>()))
            .Returns<string>(token =>
            {
                try
                {
                    _handler.ReadJsonWebToken(token);
                    return Task.CompletedTask;
                }
                catch (Exception)
                {
                    throw ApiException.Create(
                            new HttpRequestMessage(HttpMethod.Post, "http://localhost"),
                            HttpMethod.Post,
                            new HttpResponseMessage(HttpStatusCode.Unauthorized), new RefitSettings())
                        .Result;
                }
            });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_sessionManagerMock.Object);
            services.Configure<RepositoryOptions>(options =>
            {
                options.BookmarksCollectionName = "bookmarks_test";
                options.UsersCollectionName = "users_test";
                options.ProjectId = ProjectId;
            });
        });
    }

    private static string ProjectId => Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID") ??
                                       throw new InvalidOperationException(
                                           "FIRESTORE_PROJECT_ID environment variable is not set");
}