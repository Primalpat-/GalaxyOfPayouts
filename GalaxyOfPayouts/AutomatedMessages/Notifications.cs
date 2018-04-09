using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Discord.WebSocket;
using GalaxyOfPayouts.Entities;
using GalaxyOfPayouts.Utilities;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Extensions;

namespace GalaxyOfPayouts.AutomatedMessages
{
    public static class Sender
    {
        public static void Send(DiscordSocketClient client)
        {
            var currentInstant = SystemClock.Instance.GetCurrentInstant();

            var elapsedRotations = new List<Rotations>();
            using (var db = new GOPContext())
            {
                elapsedRotations = db.Rotations
                                     .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
                                     .Where(r => r.LastNotification == null ||
                                                 Instant.FromDateTimeUtc(DateTime.SpecifyKind(r.LastNotification.Value, DateTimeKind.Utc))
                                                 + Duration.FromHours(23) < currentInstant)
                                     .ToList();
            }

            foreach (var rotation in elapsedRotations)
            {
                var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(rotation.Timezone);
                var zonedDateTime = SystemClock.Instance.InZone(zone).GetCurrentZonedDateTime();
                if (zonedDateTime.Hour != 17)
                    continue;

                var channel = client.Guilds
                                    .SelectMany(g => g.TextChannels)
                                    .SingleOrDefault(c => c.Id == rotation.ChannelId);
                if (channel == null)
                    continue;

                using (channel.EnterTypingState())
                    channel.SendMessageAsync(rotation.ToFormattedMessage(true));

                using (var db = new GOPContext())
                {
                    var rotationToUpdate = db.Rotations
                                             .Single(r => r.Id == rotation.Id);
                    var ranksToUpdate = db.RotationUsers
                                          .Where(r => r.RotationId == rotation.Id)
                                          .OrderBy(r => r.NextRank)
                                          .ToList();
                    if (ranksToUpdate.Count > 0)
                    {
                        foreach (var rank in ranksToUpdate)
                            rank.NextRank--;

                        ranksToUpdate[0].NextRank = ranksToUpdate.Count;
                    }
                    rotationToUpdate.LastNotification = currentInstant.ToDateTimeUtc();
                    db.SaveChanges();
                }
            }
        }
    }
}
