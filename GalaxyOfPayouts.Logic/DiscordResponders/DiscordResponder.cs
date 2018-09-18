using System;
using System.Collections.Generic;
using System.Text;
using GalaxyOfPayouts.Logic.DiscordResponders.Behaviors;

namespace GalaxyOfPayouts.Logic.DiscordResponders
{
    public class DiscordResponder
    {
        private IResponseBehavior _responseBehavior;

        public void SetResponseBehavior(IResponseBehavior behavior)
        {
            _responseBehavior = behavior;
        }

        public void Respond()
        {
            _responseBehavior.SendResponse();
        }
    }
}
