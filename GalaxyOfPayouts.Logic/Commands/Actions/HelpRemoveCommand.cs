using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpRemoveCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation <timezone> remove <comma separated list of player(s)>:**  This will remove the player(s) from the specified timezone." + Environment.NewLine +
                   $"Leftover players will be moved up in ranks, but remain in the order they were in before removing other players.";
        }
    }
}