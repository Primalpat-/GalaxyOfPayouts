namespace GalaxyOfPayouts.Logic.Logging.Messages
{
    public sealed class LogMessage : ILogMessage
    {
        private readonly string _text;

        public LogMessage(string text)
        {
            _text = text;
        }

        public string Display()
        {
            return _text;
        }
    }
}
