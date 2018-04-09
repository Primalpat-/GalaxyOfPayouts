using System.Threading;
using Discord.WebSocket;
using NodaTime;

namespace GalaxyOfPayouts.AutomatedMessages
{
    public class Scheduler
    {
        private readonly DiscordSocketClient _client;
        private static Timer _timer;

        public void Start(object state = null)
        {
            Sender.Send(_client);

            _timer = new Timer(Start, null, (int)Duration.FromMinutes(1).TotalMilliseconds, 0);
        }

        public Scheduler(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
