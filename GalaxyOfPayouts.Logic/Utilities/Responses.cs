using System.Collections.Generic;
using Discord;
using GalaxyOfPayouts.Data.Entities;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Utilities
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

    public class RotationResponse
    {
        public string ErrorMessage { get; set; }
        public bool TimezoneValid { get; set; }
        public DateTimeZone Timezone { get; set; }
        public Rotations Rotation { get; set; }
    }

    public class UsersResponse
    {
        public UsersResponse()
        {
            Value = new List<IGuildUser>();
        }
        public string Message { get; set; }
        public List<IGuildUser> Value { get; set; }
        public bool Failure { get; set; }
    }
}