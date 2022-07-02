using Downcast.Bookmarks.API.Config;
using Downcast.Common.Error.Handler.Config;
using Downcast.Common.Logging;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureServices();
builder.ConfigureSerilog();
builder.ConfigureErrorHandlerOptions();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.ConfigureErrorHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseForwardedHeaders();

app.MapControllers();


app.Run();