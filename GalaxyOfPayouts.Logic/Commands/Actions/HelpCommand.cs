using System;
using Discord.WebSocket;
using GalaxyOfPayouts.Data;
using GalaxyOfPayouts.Logic.Commands.Actions;

namespace GalaxyOfPayouts.Logic.Commands
{
    public class HelpCommand : ICommand
    {
        public string Compute(GOPContext db, SocketMessage message)
        {
            return $"**Galaxy-of-payouts** is a bot designed to help arena shards in organizing rotation based payouts." + Environment.NewLine +
                   $"You can create rotations, which will automatically post a ranking list in the specified timezone at 5pm." + Environment.NewLine +
                   $"After the scheduled posting, it will automatically rotate the payout to the next in line." + Environment.NewLine +
                   $"In order to get help with the various subcommands of !rotation, type **!help <commandName>**" + Environment.NewLine +
                   $"The list of commands available currently are *create, get, delete, add, remove, and arrange.*";
        }
    }
}