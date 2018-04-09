namespace GalaxyOfPayouts.Utilities
{
    public static class Responses
    {
        public static string UnknownCommandResponse(string command)
        {
            return $"{command} is not a valid command.";
        }

        public static string UnknownRotationResponse(string timeZone)
        {
            return $"Unable to find a rotation for {timeZone}.";
        }

        public static string UserNotFoundResponse(string userName)
        {
            return $"Unable to find a discord user for {userName}.";
        }

        public static string InvalidTimeZoneResponse(string timeZone)
        {
            if (timeZone == string.Empty)
                return $"Invalid time zone.";

            return $"{timeZone} is an invalid time zone.";
        }
    }
}
