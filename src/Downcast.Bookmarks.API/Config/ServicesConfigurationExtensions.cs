using Downcast.Bookmarks.Manager;
using Downcast.Bookmarks.Repository;
using Downcast.Bookmarks.Repository.Options;

using Google.Api.Gax;
using Google.Cloud.Firestore;

using Microsoft.Extensions.Options;

namespace Downcast.Bookmarks.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder AddBookmarksApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBookmarksManager, BookmarksManager>();
        builder.Services.AddSingleton<IBookmarksRepository, BookmarksFirestoreRepository>();

        builder.Services.AddOptions<RepositoryOptions>()
            .Bind(builder.Configuration.GetSection(RepositoryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton(provider =>
        {
            IOptions<RepositoryOptions> options = provider.GetRequiredService<IOptions<RepositoryOptions>>();
            return new FirestoreDbBuilder
            {
                ProjectId         = options.Value.ProjectId,
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();
        });

        return builder;
    }
}