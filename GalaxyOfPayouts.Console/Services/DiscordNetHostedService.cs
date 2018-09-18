using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GalaxyOfPayouts.Console.Configuration;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservables;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservers;
using GalaxyOfPayouts.Logic.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GalaxyOfPayouts.Console.Services
{
    public class DiscordNetHostedService : IHostedService
    {
        private readonly AppConfig _config;
        private readonly ILogger _logger;
        private readonly GOPContext _db;
        private readonly LogMessageFactory _messageFactory;
        private readonly DiscordSocketClient _client;
        private readonly DiscordNetLogger _discordLogger;
        private readonly DiscordNetNotifications _discordNotifications;
        private readonly MessageReceived _messageReceived;

        public DiscordNetHostedService(AppConfig config, ILogger<DiscordNetHostedService> logger, LogMessageFactory messageFactory, GOPContext db,
            DiscordSocketClient client, DiscordNetLogger discordLogger, DiscordNetNotifications discordNotifications, MessageReceived messageReceived)
        {
            _config = config;
            _logger = logger;
            _db = db;
            _messageFactory = messageFactory;
            _client = client;
            _discordLogger = discordLogger;
            _discordNotifications = discordNotifications;
            _messageReceived = messageReceived;

            SetClientEvents();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var message = _messageFactory.CreateLogMessage("Starting...");
            _logger.LogInformation(message.Display());

            try
            {
                await _client.LoginAsync(TokenType.Bot, _config.Discord.BotToken);
                await _client.StartAsync();
                await _client.SetGameAsync("with code | !help");
            }
            catch (Exception ex)
            {
                var critical = _messageFactory.CreateLogMessage(ex);
                _logger.LogCritical(critical.Display());
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var message = _messageFactory.CreateLogMessage("Stopping...");
            _logger.LogInformation(message.Display());

            try
            {
                await _client.StopAsync();
            }
            catch (Exception ex)
            {
                var critical = _messageFactory.CreateLogMessage(ex);
                _logger.LogCritical(critical.Display());
            }
        }

        private void SetClientEvents()
        {
            _client.Log += _discordLogger.Log;
            _client.Ready += _discordNotifications.Ready;
            _client.MessageReceived += (message) => Task.Run(() => _messageReceived.ReceiveMessage(message));

            RegisterObservers();
        }

        private void RegisterObservers()
        {
            var helpObserver = new HelpObserver(_db, _messageReceived);
            var commandObserver = new CommandObserver(_db, _messageReceived);
        }
    }
}