using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Data.Entities;
using GalaxyOfPayouts.Logic.Utilities;
using NodaTime;
using Z.Core.Extensions;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class AddPlayersCommand : ICommand
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
                return $"You must supply a player or list of players to add.";

            var playerList = players.Split(',').ToList();
            var usersResult = RotationHelpers.GetDiscordUsers(message, playerList);
            if (usersResult.Failure)
                return usersResult.Message;

            UpdateStoredUsers(usersResult.Value);
            AddUsersToRotation(response.Rotation, usersResult.Value);

            return RotationHelpers.GetRotation(db, response.Rotation.Id).ToFormattedMessage();
        }

        private void UpdateStoredUsers(List<IGuildUser> users)
        {
            foreach (var user in users)
            {
                var storedUser = _db.Users.FirstOrDefault(u => u.Id == user.Id.ToDecimal());
                if (storedUser == null)
                    _db.Users.Add(new Users
                    {
                        Id = user.Id.ToDecimal(),
                        Nickname = user.Nickname,
                        Username = user.Username,
                        Mention = user.Mention
                    });
            }
            _db.SaveChanges();
        }

        private void AddUsersToRotation(Rotations rotation, List<IGuildUser> users)
        {
            var targetRank = rotation.RotationUsers.Count + 1;
            foreach (var user in users)
            {
                var savedRank = rotation.RotationUsers.FirstOrDefault(ru => ru.UserId == user.Id);
                if (savedRank != null)
                    continue;

                _db.RotationUsers.Add(new RotationUsers
                {
                    NextRank = targetRank,
                    RotationId = rotation.Id,
                    UserId = user.Id
                });
                targetRank++;
            }
            _db.SaveChanges();
        }
    }
}