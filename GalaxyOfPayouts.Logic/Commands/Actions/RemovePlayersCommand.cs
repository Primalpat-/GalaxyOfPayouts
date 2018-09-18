using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Data.Entities;
using GalaxyOfPayouts.Logic.Utilities;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class RemovePlayersCommand : ICommand
    {
        private GOPContext _db;
        private SocketMessage _message;

        public string Compute(GOPContext db, SocketMessage message)
        {
            _db = db;
            _message = message;

            var channelId = message.Channel.Id;
            var zoneText = message.Content.Split(" ")[1];
            var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneText);
            if (zone == null)
                return Responses.InvalidTimeZoneResponse(zoneText);

            var response = RotationHelpers.FindRotation(db, channelId, zone);
            if (response.Rotation == null)
                return response.ErrorMessage;

            var split = message.Content.Split(" ").Skip(3);
            var players = string.Join(" ", split);
            if (players == string.Empty)
                return $"You must supply a player or list of players to remove.";

            if (response.Rotation.RotationUsers.Count == 0)
                return $"The rotation for {zone} does not have any players to remove.";

            var playerList = players.Split(',').ToList();
            var usersResult = RotationHelpers.GetDiscordUsers(message, playerList);
            if (usersResult.Failure)
                return usersResult.Message;

            RemovePlayersFromRotation(response.Rotation, usersResult.Value);

            return RotationHelpers.GetRotation(db, response.Rotation.Id).ToFormattedMessage();
        }

        private void RemovePlayersFromRotation(Rotations rotation, List<IGuildUser> users)
        {
            var ranksToShift = 0;
            foreach (var rank in rotation.RotationUsers)
            {
                var remove = users.FindIndex(u => u.Id == rank.UserId) > -1;
                if (remove)
                {
                    _db.RotationUsers.Remove(rank);
                    ranksToShift++;
                    continue;
                }
                rank.NextRank = rank.NextRank - ranksToShift;
            }
            _db.SaveChanges();
        }
    }
}