using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpCreateCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation create <timezone>:**  This will create a new rotation in the designated timezone." + Environment.NewLine +
                   $"Timezones are based off of the standard IANA Timezones which you can find here: http://www.joda.org/joda-time/timezones.html";
        }
    }
}