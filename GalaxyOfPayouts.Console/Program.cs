using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Console.Configuration;
using GalaxyOfPayouts.Console.Services;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservables;
using GalaxyOfPayouts.Logic.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GalaxyOfPayouts.Console
{
    public class Startup
    {
        private readonly IHostBuilder _hostBuilder;

        public Startup(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }

        public IHostBuilder ConfigureApp()
        {
            return _hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Environment.CurrentDirectory);
                config.AddJsonFile("appsettings.json", optional: false);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables(prefix: "LANGUAGES_");

                if (hostContext.HostingEnvironment.IsDevelopment())
                    config.AddUserSecrets<Startup>();
            });
        }

        public IHostBuilder ConfigureServices()
        {
            return _hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();

                services.AddOptions();
                services.Configure<AppConfig>(hostContext.Configuration.GetSection("App"));
                services.AddScoped(sp => sp.GetService<IOptions<AppConfig>>().Value);

                services.AddScoped<IHostedService, DiscordNetHostedService>();
                services.AddTransient<LogMessageFactory>();
                services.AddTransient<DiscordSocketClient>();
                services.AddTransient<DiscordNetLogger>();
                services.AddTransient<JoinedGuild>();
                services.AddTransient<MessageReceived>();
            });
        }

        public IHostBuilder ConfigureLogging()
        {
            return _hostBuilder.ConfigureLogging((hostContext, config) =>
            {
                config.AddConsole();
                config.AddDebug();
            });
        }
    }
}