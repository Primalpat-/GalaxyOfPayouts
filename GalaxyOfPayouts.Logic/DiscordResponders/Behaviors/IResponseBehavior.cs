using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyOfPayouts.Logic.DiscordResponders.Behaviors
{
    public interface IResponseBehavior
    {
        Task SendResponse();
    }
}
