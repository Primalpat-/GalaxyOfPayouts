using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GalaxyOfPayouts.Console
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static IServiceProvider ServiceProvider { get; set; }
        private static ILogger<Program> _logger;

        public static async Task Main(string[] args = null)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration((config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("hostsettings.json", optional: true);
                    config.AddEnvironmentVariables(prefix: "PAYOUTS_");
                });

            var startup = new Startup(host);
            startup.ConfigureApp();
            startup.ConfigureServices();
            startup.ConfigureLogging();

            await host.RunConsoleAsync();
        }
    }
}