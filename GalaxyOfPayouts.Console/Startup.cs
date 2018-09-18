using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Console.Configuration;
using GalaxyOfPayouts.Console.Services;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservables;
using GalaxyOfPayouts.Logic.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GalaxyOfPayouts.Console
{
    public class Startup
    {
        private readonly IHostBuilder HostBuilder;

        public Startup(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public IHostBuilder ConfigureApp()
        {
            return HostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Environment.CurrentDirectory);
                config.AddJsonFile("appsettings.json", optional: false);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables(prefix: "PAYOUTS_");

                if (hostContext.HostingEnvironment.IsDevelopment())
                    config.AddUserSecrets<Startup>();
            });
        }

        public IHostBuilder ConfigureServices()
        {
            return HostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();

                services.AddOptions();
                services.Configure<AppConfig>(hostContext.Configuration.GetSection("App"));
                services.AddScoped(sp => sp.GetService<IOptions<AppConfig>>().Value);

                services.AddDbContext<GOPContext>(o =>
                    o.UseSqlServer(hostContext.Configuration.GetConnectionString("GOPConnection")));

                services.AddScoped<IHostedService, DiscordNetHostedService>();
                services.AddTransient<LogMessageFactory>();
                services.AddTransient<DiscordSocketClient>();
                services.AddTransient<DiscordNetLogger>();
                services.AddTransient<DiscordNetNotifications>();
                services.AddTransient<NotificationTimerElapsed>();
                services.AddTransient<MessageReceived>();
            });
        }

        public IHostBuilder ConfigureLogging()
        {
            return HostBuilder.ConfigureLogging((hostContext, config) =>
            {
                config.AddConsole();
                config.AddDebug();
            });
        }
    }
}