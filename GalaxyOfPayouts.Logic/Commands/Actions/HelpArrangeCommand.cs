using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpArrangeCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation <timezone> add <comma separated list of player(s)>:**  This will arrange the player(s) in the specified timezone." + Environment.NewLine +
                   $"Player(s) will be moved to the top of the ranks in the order they appear in the specified list.";
        }
    }
}