using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace GalaxyOfPayouts.Logic.Utilities
{
    public static class RotationHelpers
    {
        public static RotationResponse FindRotation(GOPContext db, decimal channelId, DateTimeZone zone)
        {
            var rotation = db.Rotations
                .Include(r => r.RotationUsers)
                .ThenInclude(ru => ru.User)
                .FirstOrDefault(r => r.ChannelId == channelId &&
                                     r.Timezone == zone.ToString());

            if (rotation == null)
                return new RotationResponse
                {
                    TimezoneValid = true,
                    Timezone = zone,
                    ErrorMessage = Responses.UnknownRotationResponse(zone.ToString())
                };

            return new RotationResponse
            {
                TimezoneValid = true,
                Timezone = zone,
                Rotation = rotation
            };
        }

        public static Rotations GetRotation(GOPContext db, int rotationId)
        {
            return db.Rotations
                .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
                .FirstOrDefault(r => r.Id == rotationId);
        }

        public static UsersResponse GetDiscordUsers(SocketMessage message, List<string> names)
        {
            var result = new UsersResponse();
            var channel = message.Channel as SocketGuildChannel;
            var discordUsers = channel.Guild.Users;

            foreach (var name in names)
            {
                var user = GetDiscordUser(discordUsers, name);
                if (user == null)
                {
                    result = new UsersResponse();
                    result.Failure = true;
                    result.Message = Responses.UserNotFoundResponse(name);
                }
                result.Value.Add(user);
            }

            return result;
        }

        public static IGuildUser GetDiscordUser(IReadOnlyCollection<IGuildUser> users, string playerName)
        {
            playerName = playerName.Trim();

            foreach (var user in users)
            {
                var nickName = user.Nickname?.Trim();
                if (string.Equals(nickName, playerName, StringComparison.OrdinalIgnoreCase))
                    return user;

                var userName = user.Username?.Trim();
                if (string.Equals(userName, playerName, StringComparison.OrdinalIgnoreCase))
                    return user;
            }

            return null;
        }
    }
}
