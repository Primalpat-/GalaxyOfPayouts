namespace GalaxyOfPayouts.Logic.Logging.Messages
{
    public class Source : ILogMessage
    {
        private readonly ILogMessage _logMessage;
        private readonly string _source;

        public Source(ILogMessage logMessage, string source)
        {
            _logMessage = logMessage;
            _source = source;
        }

        public string Display()
        {
            return $"{_source}: {_logMessage.Display()}";
        }
    }
}
