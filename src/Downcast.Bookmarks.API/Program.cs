using Downcast.Bookmarks.API.Config;
using Downcast.Common.Errors.Handler.Config;
using Downcast.Common.Logging;
using Downcast.SessionManager.SDK.Authentication.Handler;

using Microsoft.OpenApi.Models;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("http-clients-settings.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        Description  = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDowncastAuthentication(builder.Configuration);

builder.AddBookmarksApiServices();
builder.AddSerilog();
builder.AddErrorHandlerOptions();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders();

app.MapControllers();


app.Run();