
using Altair.Application;
using Altair.Middleware;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;
using Scalar.AspNetCore;

namespace Altair.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.InjectApplication();

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var serviceUrl = Environment.GetEnvironmentVariable("GOTENBERG_CLIENT_URL") ?? string.Empty;

        builder.Services.AddOptions<GotenbergSharpClientOptions>()
            .Configure(options =>
            {
                options.ServiceUrl = new Uri(serviceUrl);
                options.TimeOut = TimeSpan.FromSeconds(15);
                options.RetryPolicy = new RetryOptions
                {
                    Enabled = true,
                    RetryCount = 1,
                    BackoffPower = 1.5,
                    LoggingEnabled = true
                };
            });

        builder.Services.AddGotenbergSharpClient();

        var app = builder.Build();

        app.UseMiddleware<DefaultMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapControllers();

        app.Run();
    }
}
