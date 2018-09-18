using System;

namespace GalaxyOfPayouts.Logic.Logging.Messages
{
    public class Timestamp : ILogMessage
    {
        private readonly ILogMessage _logMessage;

        public Timestamp(ILogMessage logMessage)
        {
            _logMessage = logMessage;
        }

        public string Display()
        {
            return $"[{DateTime.Now,-19}] {_logMessage.Display()}";
        }
    }
}
