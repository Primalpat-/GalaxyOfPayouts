using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Data.Entities;
using GalaxyOfPayouts.Logic.Utilities;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Extensions;

namespace GalaxyOfPayouts.Logic.DiscordResponders.Behaviors
{
    public class NotificationBehavior : IResponseBehavior
    {
        private readonly GOPContext _db;
        private readonly DiscordSocketClient _client;

        public NotificationBehavior(GOPContext db, DiscordSocketClient client)
        {
            _db = db;
            _client = client;
        }

        public async Task SendResponse()
        {
            var current = SystemClock.Instance.GetCurrentInstant();

            var elapsedRotations = GetElapsedRotations(current);

            foreach (var rotation in elapsedRotations)
            {
                var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(rotation.Timezone);
                var zonedDateTime = SystemClock.Instance.InZone(zone).GetCurrentZonedDateTime();
                if (zonedDateTime.Hour != 17)
                    continue;

                var channel = _client.Guilds
                                     .SelectMany(g => g.TextChannels)
                                     .SingleOrDefault(c => c.Id == rotation.ChannelId);
                if (channel == null)
                    continue;

                using (channel.EnterTypingState())
                    await channel.SendMessageAsync(rotation.ToFormattedMessage(true));

                RotateRotation(rotation, current);
            }
        }

        private List<Rotations> GetElapsedRotations(Instant current)
        {
            var result = new List<Rotations>();

            result = _db.Rotations
                .Include(r => r.RotationUsers).ThenInclude(ru => ru.User)
                .Where(r => r.LastNotification == null ||
                            Instant.FromDateTimeUtc(DateTime.SpecifyKind(r.LastNotification.Value, DateTimeKind.Utc))
                            + Duration.FromHours(23) < current)
                .ToList();

            return result;
        }

        private void RotateRotation(Rotations rotation, Instant current)
        {

            var rotationToUpdate = _db.Rotations
                .Single(r => r.Id == rotation.Id);

            var ranksToUpdate = _db.RotationUsers
                .Where(r => r.RotationId == rotation.Id)
                .OrderBy(r => r.NextRank)
                .ToList();

            if (ranksToUpdate.Count > 0)
            {
                foreach (var rank in ranksToUpdate)
                    rank.NextRank--;

                ranksToUpdate[0].NextRank = ranksToUpdate.Count;
            }

            rotationToUpdate.LastNotification = current.ToDateTimeUtc();
            _db.SaveChanges();
        }
    }
}
