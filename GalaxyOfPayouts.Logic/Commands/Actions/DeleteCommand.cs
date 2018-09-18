using System.Linq;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.Utilities;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class DeleteCommand : ICommand
    {
        private GOPContext _db;
        private SocketMessage _message;

        public string Compute(GOPContext db, SocketMessage message)
        {
            _db = db;
            _message = message;

            var channelId = message.Channel.Id;
            var zoneText = message.Content.Split(" ").Last();
            var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneText);
            if (zone == null)
                return Responses.InvalidTimeZoneResponse(zoneText);

            var response = RotationHelpers.FindRotation(db, channelId, zone);
            if (response.Rotation == null)
                return response.ErrorMessage;

            db.Rotations.Remove(response.Rotation);
            db.SaveChanges();

            return $"{zone} successfully removed.";
        }
    }
}