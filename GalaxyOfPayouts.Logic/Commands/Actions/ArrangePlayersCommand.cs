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
    public class ArrangePlayersCommand : ICommand
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

            if (response.Rotation.RotationUsers.Count == 0)
                return $"You must add players to this rotation before you can arrange them.";

            var split = message.Content.Split(" ").Skip(3);
            var players = string.Join(" ", split);
            if (players == string.Empty)
                return $"You must supply a player or list of players in the order they should appear in the ranks.";

            var playerList = players.Split(',').ToList();
            var usersResult = RotationHelpers.GetDiscordUsers(message, playerList);
            if (usersResult.Failure)
                return usersResult.Message;

            ArrangeUsersInRotation(response.Rotation, usersResult.Value);

            return RotationHelpers.GetRotation(db, response.Rotation.Id).ToFormattedMessage();
        }

        private void ArrangeUsersInRotation(Rotations rotation, List<IGuildUser> users)
        {
            var oldRanks = rotation.RotationUsers.ToList();
            var newRanks = new List<RotationUsers>();

            _db.RotationUsers.RemoveRange(oldRanks);
            _db.SaveChanges();

            var incrementor = 0;
            foreach (var rank in oldRanks)
            {
                var nextRank = users.FindIndex(u => u.Id == rank.UserId);
                if (nextRank == -1)
                {
                    nextRank = users.Count + incrementor;
                    incrementor++;
                }

                rank.NextRank = nextRank + 1;
                newRanks.Add(rank);
            }

            _db.RotationUsers.AddRange(newRanks);
            _db.SaveChanges();
        }
    }
}