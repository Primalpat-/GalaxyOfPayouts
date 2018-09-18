using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservables;
using GalaxyOfPayouts.Logic.DiscordEvents.EventObservers;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace GalaxyOfPayouts.Console.Services
{
    public class DiscordNetNotifications
    {
        private readonly ILogger _logger;
        private readonly GOPContext _db;
        private readonly DiscordSocketClient _client;
        private readonly NotificationTimerElapsed _notificationTimerElapsed;
        private static Timer _timer;

        public DiscordNetNotifications(ILogger<DiscordNetLogger> logger, GOPContext db,
            DiscordSocketClient client, NotificationTimerElapsed notificationTimerElapsed)
        {
            _logger = logger;
            _db = db;
            _client = client;
            _notificationTimerElapsed = notificationTimerElapsed;

            RegisterObservers();
        }

        public Task Ready()
        {
            _timer = new Timer((int)Duration.FromMinutes(1).TotalMilliseconds);
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _logger.LogInformation("Notification timer elapsed");
            _notificationTimerElapsed.HandleNotification(_client);
        }

        private void RegisterObservers()
        {
            var notificationObserver = new NotificationObserver(_db, _notificationTimerElapsed);
        }
    }
}
