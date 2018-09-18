using System.Linq;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Data.Entities;
using GalaxyOfPayouts.Logic.Utilities;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class CreateCommand : ICommand
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
            if (!response.TimezoneValid)
                return response.ErrorMessage;

            if (response.Rotation != null)
                return $"There is already a rotation saved for {zone}";

            var rotation = new Rotations();
            rotation.ChannelId = channelId;
            rotation.Timezone = zone.ToString();

            db.Rotations.Add(rotation);
            db.SaveChanges();

            return $"Successfully created a new rotation for {zone}.";
        }
    }
}