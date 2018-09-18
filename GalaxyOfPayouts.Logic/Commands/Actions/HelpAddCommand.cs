using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpAddCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation <timezone> add <comma separated list of player(s)>:**  This will add the player(s) to the specified timezone." + Environment.NewLine +
                   $"Player(s) will be added in the order they appear in the specified list, and if you use valid discord names, it will use mentions in the scheduled posting.";
        }
    }
}