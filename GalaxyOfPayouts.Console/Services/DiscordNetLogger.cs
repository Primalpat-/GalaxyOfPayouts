using System.Threading.Tasks;
using Discord;
using GalaxyOfPayouts.Logic.Logging;
using Microsoft.Extensions.Logging;

namespace GalaxyOfPayouts.Console.Services
{
    public class DiscordNetLogger
    {
        private readonly ILogger _logger;
        private readonly LogMessageFactory _messageFactory;

        public DiscordNetLogger(ILogger<DiscordNetLogger> logger, LogMessageFactory messageFactory)
        {
            _logger = logger;
            _messageFactory = messageFactory;
        }

        public Task Log(LogMessage discordLogMessage)
        {
            var message = _messageFactory.CreateLogMessage(discordLogMessage);
            switch (discordLogMessage.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(message.Display());
                    break;
                case LogSeverity.Error:
                    _logger.LogError(message.Display());
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(message.Display());
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message.Display());
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug(message.Display());
                    break;
                default:
                    _logger.LogDebug(message.Display());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}