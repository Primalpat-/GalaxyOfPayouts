using System.Collections.Generic;
using Discord;
using NodaTime;

namespace GalaxyOfPayouts.Models
{
    public class RotationModel
    {
        public IMessageChannel Channel { get; set; }
        public DateTimeZone TimeZone { get; set; }
        public List<PlayerModel> Ranks { get; set; }
        public LocalDateTime? LastNotification { get; set; }
    }
}
