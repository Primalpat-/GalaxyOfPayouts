using GalaxyOfPayouts.Models;
using System;
using System.Linq;
using GalaxyOfPayouts.Entities;

namespace GalaxyOfPayouts.Utilities
{
    public static class RotationExtensions
    {
        public static string ToFormattedMessage(this Rotations entity, bool useMentions = false)
        {
            var users = entity.RotationUsers
                              .OrderBy(ru => ru.NextRank)
                              .Select(ru => ru.User)
                              .ToList();
            var message = $"**-- {entity.Timezone} --**";

            for (var i = 0; i < users.Count; i++)
            {
                message += Environment.NewLine;
                if (useMentions)
                    message += $"Rank {i + 1}: {users[i].Mention ?? users[i].Nickname ?? users[i].Username}";
                else
                    message += $"Rank {i + 1}: {users[i].Nickname ?? users[i].Username}";
            }

            if (users.Count == 0)
                message += $"{Environment.NewLine}No players added";

            return message;
        }
    }
}
