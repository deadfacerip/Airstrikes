using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.API;
using UnityEngine;

namespace Teyhota.Airstrikes.Commands
{
    public class Command_Position : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "position";

        public string Help => "Show your's or another player's current position";

        public string Syntax => "[player]";

        public List<string> Aliases => new List<string>() { "pos" };

        public List<string> Permissions => new List<string>() { "airstrike.position" };

        public const string PERMISSION = "airstrike.position.other";



        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                if (caller is ConsolePlayer)
                {
                    Airstrikes.Write("<player>", ConsoleColor.Red);
                    return;
                }

                UnturnedPlayer callr = (UnturnedPlayer)caller;

                UnturnedChat.Say(caller, callr.Position.ToString());

                Airstrikes.Write(callr.CharacterName + "'s Position >> " + callr.Position.ToString(), ConsoleColor.Cyan);
            }
            else
            {
                UnturnedPlayer toPlayer = UnturnedPlayer.FromName(command[0]);

                if (caller is ConsolePlayer)
                {
                    Airstrikes.Write(toPlayer.CharacterName + "'s Position >> " + toPlayer.Position.ToString(), ConsoleColor.Cyan);
                    return;
                }

                if (!caller.HasPermission(PERMISSION))
                {
                    UnturnedChat.Say(caller, "You don't have the permission to execute this command", Color.red);
                    return;
                }
                
                UnturnedChat.Say(caller, toPlayer.Position.ToString());

                Airstrikes.Write(toPlayer.CharacterName + "'s Position >> " + toPlayer.Position.ToString(), ConsoleColor.Cyan);
            }
        }
    }
}