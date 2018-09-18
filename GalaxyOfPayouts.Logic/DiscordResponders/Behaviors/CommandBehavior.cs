using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.Commands;
using GalaxyOfPayouts.Logic.Commands.Actions;

namespace GalaxyOfPayouts.Logic.DiscordResponders.Behaviors
{
    public class CommandBehavior : IResponseBehavior
    {
        private readonly GOPContext _db;
        private readonly SocketMessage _message;

        public CommandBehavior(GOPContext db, SocketMessage message)
        {
            _db = db;
            _message = message;
        }

        public async Task SendResponse()
        {
            if (FilterMessage())
                return;

            var controller = CommandController.GetInstance();
            var response = default(string);

            try
            {
                response = controller.RunCommand(_db, _message);
            }
            catch (Exception ex)
            {
                response = $"";
            }

            await _message.Channel.SendMessageAsync(response);
        }

        private bool FilterMessage()
        {
            if (_message.Source == MessageSource.Bot)
                return true;

            if (_message.Source == MessageSource.Webhook)
                return true;

            if (!_message.Content.StartsWith("!rotation", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
