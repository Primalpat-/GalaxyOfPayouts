using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GalaxyOfPayouts.Utilities;
using GalaxyOfPayouts.Modules.Rotation;

namespace GalaxyOfPayouts.Modules
{
    public class RotationModule : ModuleBase
    {
        [Command("rotation"), Summary("Handles the manipulation of arena rank rotations.")]
        public async Task Rotation([Summary("Name of the command you are invoking")] string command, 
                                   [Summary("The text you are sending to the command")] string commandText = "")
        {
            using (Context.Channel.EnterTypingState())
            {
                var commands = new RotationCommands(Context);
                var reply = Responses.UnknownCommandResponse(command);

                if (command.ToLower() == "help")
                    reply = commands.Help(commandText);

                if (command.ToLower() == "create")
                    reply = commands.Create(commandText);

                if (command.ToLower() == "get")
                    reply = commands.Get(commandText);

                if (command.ToLower() == "delete")
                    reply = commands.Delete(commandText);

                await ReplyAsync(reply);
            }
           
        }

        [Command("rotation"), Summary("Handles the manipulation of arena rank rotations.")]
        public async Task Rotation([Summary("Timezone of the rotation you are modifying")] string timeZone,
                                   [Summary("Name of the command you are invoking")] string command,
                                   [Remainder, Summary("The text you are sending to the command")] string commandText)
        {
            using (Context.Channel.EnterTypingState())
            {
                var commands = new RotationCommands(Context);
                var reply = Responses.UnknownCommandResponse(command);

                if (command.ToLower() == "add")
                    reply = commands.AddPlayers(timeZone, commandText);

                if (command.ToLower() == "remove")
                    reply = commands.RemovePlayers(timeZone, commandText);

                if (command.ToLower() == "arrange")
                    reply = commands.ArrangePlayers(timeZone, commandText);

                await ReplyAsync(reply);
            }
        }
    }
}
