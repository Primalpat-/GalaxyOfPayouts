using System;   
using System.Collections.Generic;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.Commands.Actions;
using Z.Core.Extensions;

namespace GalaxyOfPayouts.Logic.Commands
{
    public class CommandController
    {
        private static CommandController _instance;
        private readonly Dictionary<string, ICommand> _commands;

        public static CommandController GetInstance()
        {
            if (_instance.IsNull())
                _instance = new CommandController();

            return _instance;
        }

        public string RunCommand(GOPContext db, SocketMessage message)
        {
            var command = default(ICommand);

            if (message.Content.StartsWith("!help", StringComparison.OrdinalIgnoreCase))
                return _commands[message.Content.ToLower()].Compute(db, message);

            if (!message.Content.StartsWith("!rotation", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (message.Content.Contains("create", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation create"];
            if (message.Content.Contains("delete", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation delete"];
            if (message.Content.Contains("get", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation get"];
            if (message.Content.Contains("add", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation add"];
            if (message.Content.Contains("remove", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation remove"];
            if (message.Content.Contains("arrange", StringComparison.OrdinalIgnoreCase))
                command = _commands["!rotation arrange"];
            if (command.IsNull())
                command = _commands["!rotation"];

            return command.Compute(db, message);
        }

        private CommandController()
        {
            _commands = new Dictionary<string, ICommand>();
            SetCommands();
        }

        private void SetCommands()
        {
            var help = new HelpCommand();
            var helpCreate = new HelpCreateCommand();
            var helpGet = new HelpGetCommand();
            var helpDelete = new HelpDeleteCommand();
            var helpAdd = new HelpAddCommand();
            var helpRemove = new HelpRemoveCommand();
            var helpArrange = new HelpArrangeCommand();

            SetCommand("!help", help);
            SetCommand("!help create", helpCreate);
            SetCommand("!help get", helpGet);
            SetCommand("!help delete", helpDelete);
            SetCommand("!help add", helpAdd);
            SetCommand("!help remove", helpRemove);
            SetCommand("!help arrange", helpArrange);

            var rotationGet = new GetCommand();
            var rotationCreate = new CreateCommand();
            var rotationDelete = new DeleteCommand();
            var rotationAdd = new AddPlayersCommand();
            var rotationRemove = new RemovePlayersCommand();
            var rotationArrange = new ArrangePlayersCommand();

            SetCommand("!rotation", rotationGet);
            SetCommand("!rotation get", rotationGet);
            SetCommand("!rotation create", rotationCreate);
            SetCommand("!rotation delete", rotationDelete);
            SetCommand("!rotation add", rotationAdd);
            SetCommand("!rotation remove", rotationRemove);
            SetCommand("!rotation arrange", rotationArrange);
        }

        private void SetCommand(string commandString, ICommand command)
        {
            _commands.Add(commandString, command);
        }
    }
}
