using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpGetCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation get <timezone or all>:**  This will display one of or all of the stored rotations for the channel.";
        }
    }
}