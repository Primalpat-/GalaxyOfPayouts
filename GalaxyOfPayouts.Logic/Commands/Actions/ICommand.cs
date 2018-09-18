using Discord.WebSocket;
using GalaxyOfPayouts.Data;

namespace GalaxyOfPayouts.Logic.Commands.Actions
{
    public interface ICommand
    {
        string Compute(GOPContext db, SocketMessage message);
    }
}
