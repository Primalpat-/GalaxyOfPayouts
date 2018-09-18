using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public class HelpDeleteCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**!rotation delete <timezone>:**  This will remove the designated rotation.";
        }
    }
}