using System;
using System.Linq;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.Utilities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class GetCommand : ICommand
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
                return GetAll(channelId);

            var response = RotationHelpers.FindRotation(db, channelId, zone);
            if (response.Rotation == null)
                return response.ErrorMessage;

            return response.Rotation.ToFormattedMessage();
        }

        private string GetAll(decimal channelId)
        {
            var result = string.Empty;

            result = _db.Rotations
                .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
                .Where(r => r.ChannelId == channelId)
                .ToList()
                .Aggregate(string.Empty,
                    (c, r) => c + Environment.NewLine + Environment.NewLine + r.ToFormattedMessage());

            if (string.IsNullOrEmpty(result))
                return $"No rotations saved yet. Use **!help** for instructions on setting them up!";

            return result;
        }
    }
}